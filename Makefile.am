SUBDIRS = src daemon tools examples tests

test:
if ENABLE_TESTS
	$(MAKE) -C tests setup run
else
	@echo "Test are disabled, run ./configure with --enable-tests"
endif

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = dbus-sharp-2.0.pc

EXTRA_DIST = \
	SharedAssemblyInfo.cs.in \
	dbus-sharp-2.0.pc.in \
	dbus-sharp.snk \
	dbus-sharp.sln

DISTCLEANFILES = \
	SharedAssemblyInfo.cs \
	dbus-sharp-2.0.pc

MAINTAINERCLEANFILES = \
	INSTALL \
	Makefile.in \
	aclocal.m4 \
	config.guess \
	config.sub \
	configure \
	install-sh \
	missing

