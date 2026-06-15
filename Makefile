.DEFAULT_GOAL := help
.PHONY: help build test coverage pack format clean

CONFIGURATION 		?= Release
COVERAGE_DIR  		?= TestResults/coverage-results
COVERAGE_REPORT_DIR ?= TestResults/coverage-report
NUPKG_DIR     		?= nupkgs

help: ## Show this help
	@echo "DotBahn — Make targets"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*## ' $(MAKEFILE_LIST) | awk -F ':.*## ' '{printf "  %-15s %s\n", $$1, $$2}'
	@echo ""
	@echo "Variables (set via make VAR=value):"
	@echo "  CONFIGURATION   Build configuration (default: Release)"
	@echo ""

build:format ## Build all projects (Release)
	dotnet build --configuration $(CONFIGURATION)

test: build ## Run all tests with coverage
	dotnet test --no-build --configuration $(CONFIGURATION) \
		--collect:"XPlat Code Coverage" \
		--results-directory ./$(COVERAGE_DIR)

coverage: test ## Generate HTML coverage report
	dotnet tool install -g dotnet-reportgenerator-globaltool 2>/dev/null || true
	dotnet reportgenerator \
		-reports:./$(COVERAGE_DIR)/**/coverage.cobertura.xml \
		-targetdir:./$(COVERAGE_REPORT_DIR) \
		-reporttypes:Html \
		-verbosity:Warning \
		-filefilters:-*RegexGenerator.g.cs*
	@echo "Coverage report: file://$(PWD)/$(COVERAGE_REPORT_DIR)/index.html"

pack: build ## Build NuGet packages
	dotnet pack --no-build --configuration $(CONFIGURATION) \
		--output ./$(NUPKG_DIR)

format: ## Auto-fix all code formatting (style, analyzers, whitespace)
	dotnet format whitespace --verbosity diagnostic
	dotnet format --verbosity diagnostic

clean: ## Remove all build artifacts
	dotnet clean
	rm -rf ./$(COVERAGE_DIR) ./$(COVERAGE_REPORT_DIR) ./$(NUPKG_DIR)