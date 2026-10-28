Name:           dbpeek
Version:        %{?pkg_version}%{!?pkg_version:0.0.0}
Release:        0
Summary:        German railway departures monitor
License:        MIT
URL:            https://github.com/rlvelte/dotbahn
Source0:        %{name}-%{version}.tar.gz
ExclusiveArch:  x86_64
%define debug_package %{nil}

%description
One-shot departures board for Deutsche Bahn timetable services,
built with DotBahn (https://github.com/rlvelte/dotbahn).

%prep
%autosetup -n %{name}-%{version}

%build

%install
install -Dm755 dbpeek %{buildroot}%{_bindir}/dbpeek

%check
:

%files
%license LICENSE
%{_bindir}/dbpeek

%changelog
* Wed Oct 28 2026 Robin L. Velte <rlvelte@users.noreply.github.com> - 2.1.1-0
- Initial package
