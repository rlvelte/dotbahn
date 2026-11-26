# In
SOLUTION := ./DotBahn.slnx
SAMPLES_DIR := ./samples
CONFIGURATION ?= Release
NUGET_PACKAGES_DIR ?=

# Out
RESULTS_DIR := ./build/tests
BENCH_DIR := ./build/benchmarks
MUTATE_DIR := ./build/mutations
NUPKG_DIR := ./build/nupkgs
PUBLISH_DIR := ./build/publish
RPM_DIR := ./build/rpm
SBOM_DIR := ./build/sbom

# OS
ifeq ($(OS),Windows_NT)
  RUN := pwsh -NoProfile -File
  EXT := .ps1
else
  RUN := bash
  EXT := .sh
endif

# Export
export CONFIGURATION
export SOLUTION
export SAMPLES_DIR
export PUBLISH_DIR
export RPM_DIR

# Main
.DEFAULT_GOAL := build
.PHONY: format restore vulcheck sbom build test mutate pack publish rpm

format:
	dotnet format $(SOLUTION) \
		--verbosity diagnostic

restore:
	dotnet restore $(SOLUTION) \
		$(if $(NUGET_PACKAGES_DIR),--packages $(NUGET_PACKAGES_DIR),) \
		--force-evaluate \
		--verbosity minimal

vulcheck:
	dotnet list $(SOLUTION) package \
		--vulnerable \
		--include-transitive

sbom:
	dotnet cyclonedx $(SOLUTION) \
		-o $(SBOM_DIR) \
		-F Json

build: restore
	dotnet build $(SOLUTION) \
		--configuration $(CONFIGURATION) \
		--verbosity minimal

test: build
	dotnet test --solution $(SOLUTION) \
		--configuration $(CONFIGURATION) \
		--results-directory $(RESULTS_DIR) \
		--no-build \
		-- --coverage --coverage-output-format cobertura

mutate: export MinVerVersionOverride = 1.0.0
mutate: build
	mkdir -p $(MUTATE_DIR)
	dotnet stryker \
		--output $(MUTATE_DIR)

pack: build
	dotnet pack $(SOLUTION) \
		--configuration $(CONFIGURATION) \
		--output $(NUPKG_DIR) \
		--no-build \
		--verbosity minimal

publish:
	$(RUN) scripts/publish$(EXT)

rpm:
ifeq ($(OS),Windows_NT)
	@echo "Windows does not support 'rpmbuild'." >&2
else
	$(RUN) scripts/rpm$(EXT)
endif

clean:
	dotnet clean $(SOLUTION) \
		--configuration $(CONFIGURATION) \
		--verbosity minimal
ifeq ($(OS),Windows_NT)
	powershell -NoProfile -Command "Remove-Item -Recurse -Force -ErrorAction SilentlyContinue build, docs/_site, docs/api"
else
	rm -rf build docs/_site docs/api
endif
