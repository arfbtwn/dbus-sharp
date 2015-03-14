ASSEMBLY = dbus-daemon
TARGET = $(ASSEMBLY).exe
XBUILD_Q = $(XBUILD) /nologo /verbosity:quiet

CSFILES = \
	Server.cs \
	ServerBus.cs \
	Daemon.cs

all: $(TARGET)

$(TARGET):
	$(XBUILD_Q) $(ASSEMBLY).csproj

clean:
	$(XBUILD_Q) /target:Clean $(ASSEMBLY).csproj

# This is not needed on that branch
# it's for compat with wrench
setup: ;
run: check

.PHONY: $(TARGET)

EXTRA_DIST = \
	$(CSFILES) \
	$(ASSEMBLY).csproj

CLEANFILES = \
	$(TARGET) \
	$(TARGET).mdb

MAINTAINERCLEANFILES = \
	Makefile.in
