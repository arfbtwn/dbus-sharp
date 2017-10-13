m4_define([api_version], [2.0])
m4_define([lib_version], [0.9])

AC_PREREQ(2.13)

AC_INIT([dbus-sharp], lib_version)
AC_CONFIG_MACRO_DIRS([m4])
AC_CONFIG_SRCDIR([src/DBus.cs])
AM_INIT_AUTOMAKE

AM_MAINTAINER_MODE

AC_SUBST([API_VERSION], [api_version])
AC_SUBST([VERSION], [lib_version])

AC_PROG_INSTALL

AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
	AC_MSG_ERROR([You need to install pkg-config])
fi
AC_SUBST(PKG_CONFIG)

MONO_REQ_VERSION=1.1.13
MONO_INIT([mono >= ${MONO_REQ_VERSION}])
MONO_INIT_XBUILD
MONO_INIT_GACUTIL
NUGET_INIT

required_assemblies="Mono.Posix"

for asm in $required_assemblies; do
	AC_MSG_CHECKING([for $asm.dll])
	if test -n "`$GACUTIL /l $asm.dll | grep ^$asm.dll`"; then
		AC_MSG_RESULT([not found])
		AC_MSG_ERROR([missing required assembly in the GAC: $asm.dll])
	else
		AC_MSG_RESULT([found])
	fi
done

NUNIT_REQUIRED=2.6.4

AC_ARG_ENABLE(tests, AC_HELP_STRING([--enable-tests], [Enable NUnit tests]),
	enable_tests=$enableval, enable_tests="no")

if test "x$enable_tests" = "xno"; then
	do_tests=no
	AM_CONDITIONAL(ENABLE_TESTS, false)
else
	NUGET_INSTALL([NUnit],[${NUNIT_REQUIRED}],[do_tests="yes"])
	NUGET_INSTALL([NUnit.ConsoleRunner],[3.7.0])
	NUGET_INSTALL([NUnit.Extension.NUnitV2Driver],[3.7.0])

	MONO_PROG([RUNNER],[${NUGET_DIR}NUnit.ConsoleRunner.3.7.0/tools/nunit3-console.exe])
	AC_SUBST([RUNNER])

	AM_CONDITIONAL(ENABLE_TESTS, test "x$do_tests" = "xyes")
fi

AC_OUTPUT([
Makefile
dbus-sharp-2.0.pc
src/AssemblyInfo.cs
src/Makefile
tools/Makefile
examples/Makefile
tests/Makefile
])

