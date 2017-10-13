# ================================================
#
# SYNOPSIS
#
#   MONO_INIT()
#
#   $1 := Mono package check
#
# ================================================
AC_DEFUN([MONO_INIT],
[
    AC_PROVIDE([$0])

    PKG_CHECK_MODULES([MONO],[m4_default_quoted([$1],[mono])])

    AC_PATH_PROG([MONO], [mono], [no])
    AS_IF([test "x${MONO}" = "xno"],[AC_MSG_ERROR([Couldn't find mono in your PATH])])
])

AC_DEFUN([MONO_INIT_XBUILD],
[
    AC_PATH_PROG([XBUILD], [xbuild], [no])
    AS_IF([test "x${XBUILD}" = "xno"],[AC_MSG_ERROR([Couldn't find xbuild in your PATH])])
])

AC_DEFUN([MONO_INIT_MCS],
[
    AC_PATH_PROG([MCS], [mcs], [no])
    AS_IF([test "x${MCS}" = "xno"],[AC_MSG_ERROR([Couldn't find mcs in your PATH])])
])

AC_DEFUN([MONO_INIT_GACUTIL],
[
    AC_PATH_PROG([GACUTIL], [gacutil], [no])
    AS_IF([test "x${GACUTIL}" = "xno"],[AC_MSG_ERROR([Couldn't find gacutil in your PATH])])
])

# ================================================
#
# SYNOPSIS
#
#   MONO_PROG()
#
#   $1 := VARIABLE
#   $2 := PROGRAM
#
# ================================================
AC_DEFUN([MONO_PROG],
[
    AC_REQUIRE([MONO_INIT])

    AC_SUBST($1,["${MONO} $2"])
])
