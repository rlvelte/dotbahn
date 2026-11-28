Name:           dbpeek
Version:        %{?pkg_version}%{!?pkg_version:0.0.0}
Release:        0
Summary:        German railway arrival/departure board
License:        MIT
URL:            https://github.com/rlvelte/dotbahn
Source0:        %{name}-%{version}.tar.gz
ExclusiveArch:  x86_64
%define debug_package %{nil}

%description
One-shot arrival and departure board for Deutsche Bahn, built with
DotBahn (https://github.com/rlvelte/dotbahn) using the OpenBoards API.

%prep
%autosetup -n %{name}-%{version}

%build

%install
install -Dm755 dbpeek %{buildroot}%{_bindir}/dbpeek
install -Dm644 bom.json %{buildroot}%{_datadir}/%{name}/bom.json

%check
:

%files
%license LICENSE
%{_bindir}/dbpeek
%{_datadir}/%{name}/bom.json

%changelog
* Sun Nov 22 2026 Robin L. Velte <rlvelte@users.noreply.github.com> - 2.2.0-0
- Migrate to the new OpenBoards API (DotBahn.Boards)
- Add arrivals mode (--arrivals/-a)
- Support departure/arrival filtering by category, limit and time window
- Cap --window to 8 hours per API limit
* Wed Oct 28 2026 Robin L. Velte <rlvelte@users.noreply.github.com> - 2.1.1-0
- Initial package
