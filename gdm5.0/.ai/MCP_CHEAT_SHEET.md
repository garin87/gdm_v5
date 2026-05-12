# MCP Integration Cheat Sheet

## ?? Quick Commands

### Setup (One-Time)
```bash
# 1. Create GitHub token at:
https://github.com/settings/tokens
# Scope: public_repo (read-only)

# 2. Add to AI config (see locations below)

# 3. Restart AI client
```

---

## ?? Config File Locations

### Claude Desktop
```
Windows: %APPDATA%\Claude\claude_desktop_config.json
Mac:     ~/Library/Application Support/Claude/claude_desktop_config.json
Linux:   ~/.config/Claude/claude_desktop_config.json
```

### GitHub Copilot
```
Command Palette ? "GitHub Copilot: Edit MCP Configuration"
```

### Cursor
```
Settings ? Features ? Tools & MCP
```

---

## ?? Config Template

```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github"],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "your_token_here"
      }
    }
  }
}
```

---

## ?? Example Prompts

### List Issues
```
List all open issues in garin87/gdm_v5
```

### Get Specific Issue
```
Show me issue #15 with all comments
```

### Search Issues
```
Find all authentication-related issues
```

### Plan Work
```
What issues are in the v2.0 milestone?
```

### Generate with Context
```
Implement the feature from issue #10 following our patterns
```

---

## ?? Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| Token invalid | Check scope is `public_repo`, verify not expired |
| MCP not found | Install Node.js, test: `npx -y @modelcontextprotocol/server-github` |
| AI not using MCP | Restart client, be explicit: "Use GitHub MCP..." |
| JSON error | Validate syntax at jsonlint.com |

---

## ?? Security Checklist

- ? Use `public_repo` scope only
- ? Never commit token to git
- ? Store in AI config only
- ? Rotate every 90 days
- ? Revoke at: https://github.com/settings/tokens

---

## ?? Full Documentation

- **Quick Start:** `.ai/MCP_QUICK_START.md` (5 min)
- **Full Guide:** `docs/MCP_INTEGRATION_GUIDE.md`
- **Config Template:** `.ai/mcp-config-template.json`

---

## ?? Three Core Capabilities

1. **Find/List Issues** ? Search and filter
2. **Get Issue Details** ? Full information with metadata
3. **Get Discussion** ? Comments and conversation

---

## ? Benefits

- ?? Find issues without leaving IDE
- ?? Get requirements instantly
- ?? Read discussions in context
- ?? Plan work based on issues
- ?? Generate code with issue context
- ?? Save ~15-20 min/day

---

**Repository:** garin87/gdm_v5  
**Setup Time:** 5 minutes  
**Status:** Ready to use! ?
