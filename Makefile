.DEFAULT_GOAL := help
.PHONY: build test coverage pack format clean docs docs-serve docs-clean

CONFIGURATION 		?= Release
COVERAGE_DIR  		?= TestResults/coverage-results
COVERAGE_REPORT_DIR ?= TestResults/coverage-report
NUPKG_DIR     		?= nupkgs
DOCFX_DIR     		?= docs

build: ## Build all projects (Release)
	dotnet build --configuration $(CONFIGURATION)

test: build ## Run all tests with coverage
	dotnet test --no-build --configuration $(CONFIGURATION) \
		--collect:"XPlat Code Coverage" \
		--results-directory ./$(COVERAGE_DIR)

coverage: test ## Generate HTML coverage report
	dotnet reportgenerator \
		-reports:./$(COVERAGE_DIR)/**/coverage.cobertura.xml \
		-targetdir:./$(COVERAGE_REPORT_DIR) \
		-reporttypes:Html \
		-verbosity:Warning \
		-filefilters:-*RegexGenerator.g.cs*
	@echo "Coverage report: file://$(PWD)/$(COVERAGE_REPORT_DIR)/index.html"

pack: build ## Build NuGet packages
	dotnet pack --no-restore --configuration $(CONFIGURATION) \
		--output ./$(NUPKG_DIR)

format: ## Auto-fix all code formatting (style, analyzers, whitespace)
	dotnet format whitespace --verbosity diagnostic
	dotnet format --verbosity diagnostic

clean: ## Remove all build artifacts
	dotnet clean
	rm -rf ./$(COVERAGE_DIR) ./$(COVERAGE_REPORT_DIR) ./$(NUPKG_DIR) ./$(DOCFX_DIR)/_site ./$(DOCFX_DIR)/api

docs: build ## Build documentation site
	dotnet docfx $(DOCFX_DIR)/docfx.json

docs-serve: build ## Build and serve documentation locally (http://localhost:8080)
	dotnet docfx $(DOCFX_DIR)/docfx.json --serve
