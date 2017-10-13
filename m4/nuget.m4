# ================================================
#
# SYNOPSIS
#
#   NUGET_INIT()
#
#   $1 := Tool install root    (default: .nuget)
#   $2 := Tool version         (default: latest)
#   $3 := Package install root (default: packages)
#
# DESCRIPTION
#
#   Initialises NuGet related configury, requires
#   mono.m4 and wget if the nuget.exe can't be
#   found at the tool install root.
#
# ================================================
AC_DEFUN([NUGET_INIT],
[
    AC_REQUIRE([MONO_INIT])
    AC_PROVIDE([$0])

    _NUGET_ROOT([m4_default_quoted([$1],[.nuget])])
    _NUGET_EXE([m4_default_quoted([$2],[latest])])

    _NUGET_ROOT_P([m4_default_quoted([$3],[packages])])

    AC_ARG_VAR([NUGET_OPTS],[Default options to pass to NuGet])
    AC_SUBST([NUGET_OPTS],["-NonInteractive -Verbosity quiet"])

    AC_SUBST([NUGET_DIR],["\$(top_builddir)/${NUGET_ROOT_P}/"])
])

AC_DEFUN([_NUGET_DIR],
[
    AC_REQUIRE([AC_PROG_MKDIR_P])

    AS_IF([test ! -d $2],
    [
        ${MKDIR_P} $2
    ])

    AC_SUBST([$1],[$2])
])

AC_DEFUN([_NUGET_ROOT],
[
    _NUGET_DIR([NUGET_ROOT],[$1])
])

AC_DEFUN([_NUGET_ROOT_P],
[
    _NUGET_DIR([NUGET_ROOT_P],[$1])
])

AC_DEFUN([_NUGET_EXE],
[
    AS_IF([test ! -f ${NUGET_ROOT}/nuget.exe],
    [
        AC_PATH_PROG([WGET],[wget])

        uri=https://dist.nuget.org/win-x86-commandline/$1/nuget.exe
        result=`${WGET} -O ${NUGET_ROOT}/nuget.exe ${uri} 2>&1`

        AS_IF([test 0 -ne $?],
        [
            rm -f ${NUGET_ROOT}/nuget.exe

            AC_MSG_ERROR([Unable to download nuget.exe:

${result}])
        ])
    ])

    chmod +x ${NUGET_ROOT}/nuget.exe

    AC_PATH_PROG([NUGET_EXE],[nuget.exe],,[${NUGET_ROOT}])
    AC_SUBST([NUGET],["${MONO} \$(top_builddir)/${NUGET_EXE}"])
])

# ================================================
#
# SYNOPSIS
#
#   NUGET_INSTALL()
#
#   $1 := PACKAGE
#   $2 := VERSION
#   $3 := [ACTION-IF-FOUND]
#   $4 := [ACTION-IF-NOT-FOUND]
#
# DESCRIPTION
#
#   Installs a NuGet package
#
# ================================================
AC_DEFUN([NUGET_INSTALL],
[
    AC_REQUIRE([NUGET_INIT])

    m4_if([$1],,[AC_MSG_ERROR([Requires a package name])])
    m4_if([$2],,[AC_MSG_ERROR([Requires a package version])])

    AC_MSG_CHECKING([for $1])
    result=`${MONO} ${NUGET_EXE} install ${NUGET_OPTS} -OutputDirectory ${NUGET_ROOT_P} $1 -Version $2 2>&1`

    AS_IF([test 0 -eq $?],
    [
        AC_MSG_RESULT([yes])

        $3
    ],
    [
        AC_MSG_RESULT([no])

        m4_if([$4],,
        [
            AC_MSG_ERROR([NuGet package requirements ($1) were not met:

${result}])
        ],
        [
            $4
        ])
    ])
])
