Name:           dbstation
Version:        %{?pkg_version}%{!?pkg_version:0.0.0}
Release:        0
Summary:        Interactive German railway station browser
License:        MIT
URL:            https://github.com/rlvelte/dotbahn
Source0:        %{name}-%{version}.tar.gz
ExclusiveArch:  x86_64
%define debug_package %{nil}

%description
Interactive browser and search for Deutsche Bahn stations and facilities,
built with DotBahn (https://github.com/rlvelte/dotbahn).

%prep
%autosetup -n %{name}-%{version}

%build

%install
install -Dm755 dbstation %{buildroot}%{_bindir}/dbstation

%check
:

%files
%license LICENSE
%{_bindir}/dbstation

%changelog
* Wed Oct 28 2026 Robin L. Velte <rlvelte@users.noreply.github.com> - 2.1.1-0
- Initial package
