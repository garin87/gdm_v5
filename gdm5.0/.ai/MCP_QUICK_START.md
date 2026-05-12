# MCP Integration Quick Start

## ? Quick Setup (5 Minutes)

This guide gets you connected to GitHub Issues through MCP so you can retrieve task information directly in your IDE.

---

## Step 1: Create GitHub Token (2 min)

1. Go to https://github.com/settings/tokens
2. Click **"Generate new token (classic)"**
3. Set name: `AI MCP Integration`
4. Select scope: ? **`public_repo`** (read-only)
5. Click **"Generate token"**
6. **Copy the token** (you won't see it again!)

---

## Step 2: Configure Your AI Client (2 min)

### For Claude Desktop

1. **Find config file:**
   - Windows: `%APPDATA%\Claude\claude_desktop_config.json`
   - Mac: `~/Library/Application Support/Claude/claude_desktop_config.json`
   - Linux: `~/.config/Claude/claude_desktop_config.json`

2. **Edit config** (create if doesn't exist):
   ```json
   {
     "mcpServers": {
       "github": {
         "command": "npx",
         "args": ["-y", "@modelcontextprotocol/server-github"],
         "env": {
           "GITHUB_PERSONAL_ACCESS_TOKEN": "paste_your_token_here"
         }
       }
     }
   }
   ```

3. **Save and restart Claude Desktop**

### For GitHub Copilot (VS Code)

1. Open **Command Palette** (Ctrl+Shift+P / Cmd+Shift+P)
2. Type: `GitHub Copilot: Edit MCP Configuration`
3. Add the same JSON as above
4. Save and restart VS Code

### For Cursor

1. Open **Settings** (Ctrl+, / Cmd+,)
2. Go to **Features ? Tools & MCP**
3. Click **"Add Server"**
4. Select **"GitHub"** or add manually
5. Paste your token
6. Save and restart Cursor

---

## Step 3: Test It (1 min)

Ask your AI:

```
List all open issues in garin87/gdm_v5
```

Or:

```
Show me issue #1 from garin87/gdm_v5 with all its comments
```

**? Success:** AI retrieves and displays GitHub issues!

---

## What You Can Now Do

### ?? Find Issues
```
Show me all authentication-related issues
```

### ?? Get Context
```
What are the requirements for issue #23?
```

### ?? Read Discussion
```
Show me the conversation on issue #15
```

### ?? Plan Work
```
What issues are blocking the v2.0 milestone?
```

### ?? Generate with Context
```
Implement the feature from issue #10 following our patterns
```

---

## Troubleshooting

### ? "Token invalid"
- Check token is copied correctly
- Verify it has `public_repo` scope
- Test: `curl -H "Authorization: token YOUR_TOKEN" https://api.github.com/user`

### ? "MCP server not found"
- Ensure Node.js is installed
- Run: `npx -y @modelcontextprotocol/server-github`
- Check internet connection

### ? "AI doesn't use GitHub"
- Restart your AI client
- Be explicit: "Use GitHub MCP to list issues"
- Check config JSON is valid (no syntax errors)

---

## Security Notes

? **Read-only token** - Uses `public_repo` scope only  
? **Never commit token** - Keep it in AI config only  
? **Rotate regularly** - Update every 90 days  
? **Revoke if compromised** - https://github.com/settings/tokens  

---

## For Client Projects

?? **Before enabling on client work:**
1. Confirm GitHub API access is approved
2. Verify AI client is approved for project
3. Use read-only access only
4. Document in security log

**Alternative for strict environments:** Copy-paste issue content manually

---

## Next Steps

- ? **Setup complete?** Test with a few queries
- ?? **Detailed guide:** See `docs/MCP_INTEGRATION_GUIDE.md`
- ?? **Advanced usage:** Configure Linear, Jira, or Asana (see guide)
- ?? **Team setup:** Share `.ai/mcp-config-template.json` with team

---

## Files Reference

- **Quick Start:** `.ai/MCP_QUICK_START.md` (this file)
- **Detailed Guide:** `docs/MCP_INTEGRATION_GUIDE.md`
- **Config Template:** `.ai/mcp-config-template.json`
- **Project Map:** `PROJECT_MAP.md`

---

**Repository:** https://github.com/garin87/gdm_v5  
**Questions?** See `docs/MCP_INTEGRATION_GUIDE.md`

**Happy issue tracking without leaving your IDE! ??**
