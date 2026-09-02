.PHONY: build test coverage pack publish-aot format clean mutate docs docs-serve

CONFIGURATION ?= Release
AOT           ?= true
NUPKG_DIR     ?= nupkgs
PUBLISH_DIR   ?= publish
DOCFX_DIR     ?= docs
TESTS_DIR  	  ?= TestResults
STRYKER_DIR   ?= StrykerOutput

build: ## Build all projects
	dotnet build --configuration $(CONFIGURATION) -p:PublishAot=$(AOT) -p:IsAotCompatible=$(AOT) -p:EnableTrimAnalyzer=$(AOT) -p:IsTrimmable=$(AOT)

test: build ## Run all tests with coverage
	dotnet test --no-build --configuration $(CONFIGURATION) \
		--collect:"XPlat Code Coverage" \
		--results-directory ./$(TESTS_DIR)

coverage: test ## Generate HTML coverage report
	dotnet reportgenerator \
		-reports:./$(TESTS_DIR)/**/coverage.cobertura.xml \
		-targetdir:./$(TESTS_DIR) \
		-reporttypes:Html \
		-verbosity:Warning \
		-filefilters:-**/*.g.cs
	@echo "Coverage report: file://$(CURDIR)/$(TESTS_DIR)/index.html"

pack: build ## Build NuGet packages
	dotnet pack --no-restore --configuration $(CONFIGURATION) -p:PublishAot=$(AOT) -p:IsAotCompatible=$(AOT) -p:EnableTrimAnalyzer=$(AOT) -p:IsTrimmable=$(AOT) \
		--output ./$(NUPKG_DIR)

format: ## Auto-fix all code formatting (style, analyzers, whitespace)
	dotnet format whitespace --verbosity diagnostic
	dotnet format --verbosity diagnostic

clean: ## Remove all build artifacts
	dotnet clean
	rm -rf ./$(STRYKER_DIR) ./$(TESTS_DIR) ./$(NUPKG_DIR) ./$(PUBLISH_DIR) ./$(DOCFX_DIR)/_site ./$(DOCFX_DIR)/api

mutate: build ## Run Stryker mutation testing
	dotnet stryker

docs: build ## Build documentation site
	dotnet docfx $(DOCFX_DIR)/docfx.json

docs-serve: build ## Build and serve documentation locally (http://localhost:8080)
	dotnet docfx $(DOCFX_DIR)/docfx.json --serve