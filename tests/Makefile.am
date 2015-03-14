ASSEMBLY = dbus-sharp-tests
TARGET = $(ASSEMBLY).dll

NUNIT_CONSOLE = $$(echo $$(which nunit-console2 || which nunit-console))
RUNNER = $(NUNIT_CONSOLE) -nologo

CSFILES = Unit/AddressTest.cs \
	Unit/AuthenticationTest.cs \
	Unit/MatchRuleTest.cs \
	Unit/ObjectPathTest.cs \
	Unit/SignatureTest.cs \
	Unit/BusTests.cs \
	Integration/ExportInterfaceTest.cs \
	Integration/RenamedInterfaceTest.cs \
	Unit/MessageReaderTest.cs \
	Unit/MessageWriterTest.cs \
	Integration/IntrospectorTest.cs

if ENABLE_TESTS

test: $(TARGET)
	if test -z "$$DBUS_SESSION_BUS_ADDRESS" ; then \
		eval `dbus-launch --sh-syntax` ; \
		echo "D-Bus per-session daemon address is: $$DBUS_SESSION_BUS_ADDRESS" ; \
	fi; $(RUNNER) $$TEST_HARNESS_FLAGS $(TARGET);

check: test

include ../Makefile.include

endif

