#
# Various commands for the Engineering platform itself
#
.PHONY: release/requirements eng/update

release/requirements:
	@ eng/release_requirements

eng/update: -check-command-curl
	@ (temp_file=$(shell mktemp); \
		curl -o "$$temp_file" -sL https://github.com/Carbonfrost/eng-commons-dotnet/archive/master.zip; \
		tar -xvf "$$temp_file" --keep-newer-files --strip-components=1 'eng-commons-dotnet-master/eng/*'; \
		tar -xvf "$$temp_file" --keep-newer-files --strip-components=2 'eng-commons-dotnet-master/integration/*'; \
	)
