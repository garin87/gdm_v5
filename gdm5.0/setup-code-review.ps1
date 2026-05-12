# Complete AI Code Review Setup Script

# Step 1: Commit configuration files
Write-Host "?? Setting up AI Code Review..." -ForegroundColor Cyan

cd D:\Dev\Ggm\gdm_v5

# Add all new config files
git add .coderabbit.yaml
git add .github/workflows/*.yml

git commit -m "Setup AI code review automation

- CodeRabbit configuration for comprehensive review
- GitHub Copilot code review workflow
- SonarCloud integration for security & quality
- Custom OpenAI GPT-4 review workflow

Focus areas:
- .NET 8 best practices
- Security vulnerabilities
- Performance issues
- Authentication changes (Identity Server removal)
- Missing tests
- Breaking changes"

# Step 2: Push to GitHub
Write-Host "`n?? Pushing to GitHub..." -ForegroundColor Cyan
git push origin feature/dotnet8-migration

Write-Host "`n? Configuration pushed!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Install CodeRabbit: https://coderabbit.ai" -ForegroundColor White
Write-Host "2. Add GitHub secrets:" -ForegroundColor White
Write-Host "   - SONAR_TOKEN (from https://sonarcloud.io)" -ForegroundColor Gray
Write-Host "   - OPENAI_API_KEY (optional, from https://platform.openai.com)" -ForegroundColor Gray
Write-Host "3. Create Pull Request" -ForegroundColor White
Write-Host "   https://github.com/garin87/gdm_v5/pull/new/feature/dotnet8-migration" -ForegroundColor Cyan

Write-Host "`n?? AI Code Review will automatically run on your PR!" -ForegroundColor Green
