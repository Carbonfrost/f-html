.PHONY: \
	-dotnet/test \
	dotnet/cover \
	dotnet/generate
	dotnet/test \

-include eng/Makefile

## Generate generated code
dotnet/generate:
	$(Q) srgen -c Carbonfrost.Commons.Html.Resources.SR \
		-r Carbonfrost.Commons.Html.Automation.SR \
		--resx \
		dotnet/src/Carbonfrost.Commons.Html/Automation/SR.properties

## Execute dotnet unit tests
dotnet/test: dotnet/publish -dotnet/test

-dotnet/test:
	$(Q) fspec $(FSPEC_OPTIONS) -i dotnet/test/Carbonfrost.UnitTests.Html/Content \
		dotnet/test/Carbonfrost.UnitTests.Html/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.Html.dll

## Run unit tests with code coverage
dotnet/cover: dotnet/publish -check-command-coverlet
	$(Q) coverlet \
		--target "make" \
		--targetargs "-- -dotnet/test" \
		--format lcov \
		--output lcov.info \
		--exclude-by-attribute 'Obsolete' \
		--exclude-by-attribute 'GeneratedCode' \
		--exclude-by-attribute 'CompilerGenerated' \
		dotnet/test/Carbonfrost.UnitTests.Html/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.Html.dll
