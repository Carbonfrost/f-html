.PHONY: dotnet/test dotnet/cover

## Generate generated code
dotnet/generate:
	srgen -c Carbonfrost.Commons.Html.Resources.SR \
		-r Carbonfrost.Commons.Html.Automation.SR \
		--resx \
		dotnet/src/Carbonfrost.Commons.Html/Automation/SR.properties

## Execute dotnet unit tests
dotnet/test: dotnet/publish -dotnet/test

-dotnet/test:
	fspec -i dotnet/test/Carbonfrost.UnitTests.Html/Content \
		dotnet/test/Carbonfrost.UnitTests.Html/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.Core.dll \
		dotnet/test/Carbonfrost.UnitTests.Html/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.Core.Runtime.Expressions.dll \
		dotnet/test/Carbonfrost.UnitTests.Html/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.Html.dll \
		dotnet/test/Carbonfrost.UnitTests.Html/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.Html.dll

## Run unit tests with code coverage
dotnet/cover: dotnet/publish -check-command-coverlet
	coverlet \
		--target "make" \
		--targetargs "-- -dotnet/test" \
		--format lcov \
		--output lcov.info \
		--exclude-by-attribute 'Obsolete' \
		--exclude-by-attribute 'GeneratedCode' \
		--exclude-by-attribute 'CompilerGenerated' \
		dotnet/test/Carbonfrost.UnitTests.Html/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.Html.dll

include eng/.mk/*.mk
