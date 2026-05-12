# AI Configuration Reference

## For All AI Development Tools

This repository has comprehensive architectural documentation designed to improve AI-assisted development.

### Primary Documentation

**? For detailed architecture, see [PROJECT_MAP.md](../PROJECT_MAP.md)**

This is the **single source of truth** for:
- Project structure and organization
- Technology stack specifications
- Architectural patterns and conventions
- Domain model relationships
- Development workflows
- Code navigation guidance

### Claude Code Integration

If you're using Claude Code, the following files provide automatic context:

- **`.claude/CLAUDE.md`** - Always-on project guidance
- **`.claude/skills/Cartograph/SKILL.md`** - Repository mapping skill

### For Other AI Tools

If you're using GitHub Copilot, Cursor, Tabnine, or other AI coding assistants:

1. **Review** `PROJECT_MAP.md` before starting work
2. **Reference** architectural patterns documented there
3. **Follow** established conventions and code structure
4. **Consult** `.claude/CLAUDE.md` for quick reference guides

### Key Principles

All AI tools working with this codebase should:

- ? Maintain architectural consistency
- ? Follow existing patterns (Service Layer, DTO, Filter patterns)
- ? Use established response formats (`ApplicationResponseGeneric<T>`)
- ? Preserve the layered architecture structure
- ? Match existing code style and conventions

### Quick Navigation

| Need | Location |
|------|----------|
| Architecture Overview | `PROJECT_MAP.md` |
| Quick Reference | `.claude/CLAUDE.md` |
| Repository Mapping | `.claude/skills/Cartograph/SKILL.md` |
| Backend Config | `Startup.cs`, `appsettings.json` |
| Frontend Config | `ClientApp/angular.json` |
| Database Schema | `Models/DataContext.cs` |

---

**Last Updated:** [Auto-generated on creation]  
**Maintained By:** Project Team Lead
