# Packages all '*.spec' in the 'SAMPLES_DIR' as rpm.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$root"

samples_dir="${SAMPLES_DIR:-./samples}"
publish_dir="${PUBLISH_DIR:-./build/publish}"
rpm_dir="${RPM_DIR:-./build/rpm}"

version="${1:-$(git describe --tags --abbrev=0 | sed 's/^v//')}"
work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

detect_rid() {
  local os arch
  case "$(uname -s)" in
    Linux*) os=linux ;; # really need this?
    *) echo "unsupported os: $(uname -s)" >&2; exit 1 ;;
  esac
  case "$(uname -m)" in
    x86_64|amd64) arch=x64 ;;
    aarch64|arm64) arch=arm64 ;;
    *) echo "unsupported arch: $(uname -m)" >&2; exit 1 ;;
  esac
  echo "${os}-${arch}"
}

rid="$(detect_rid)"

shopt -s nullglob
specs=("$samples_dir"/*/*.spec)
if [ ${#specs[@]} -eq 0 ]; then
  echo "No spec files found in $samples_dir/*/" >&2
  exit 1
fi

mkdir -p "$work/SOURCES"

for spec in "${specs[@]}"; do
  name="$(sed -n 's/^Name:[[:space:]]*//p' "$spec" | head -1)"
  if [ -z "$name" ]; then
    echo "Skipping $spec: no name field" >&2
    continue
  fi

  sample_dir="$(dirname "$spec")"
  csproj="$(basename "$(ls "$sample_dir"/*.csproj)" .csproj)"
  binary="$publish_dir/$rid/$csproj/$name"
  if [ ! -x "$binary" ]; then
    echo "Skipping $spec: missing binaries" >&2
    exit 1
  fi

  mkdir "$work/$name-$version"
  cp "$binary" "$work/$name-$version/"
  cp LICENSE "$work/$name-$version/"
  dotnet cyclonedx "$sample_dir/$csproj.csproj" \
    -o "$work/$name-$version" \
    -F Json \
    -fn bom.json \
    -dpr
  if [ ! -f "$work/$name-$version/bom.json" ]; then
    echo "Skipping $spec: sbom generation failed" >&2
    exit 1
  fi

  tar -czf "$work/SOURCES/$name-$version.tar.gz" -C "$work" "$name-$version"

  rpmbuild -bb \
    --define "_sourcedir $work/SOURCES" \
    --define "_rpmdir $rpm_dir" \
    --define "pkg_version $version" \
    "$spec"
done
