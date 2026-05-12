# MCP Integration Guide - GitHub Issues

## Overview

This guide sets up Model Context Protocol (MCP) integration with GitHub Issues for the GDM 5.0 project.

## Prerequisites

- GitHub account with access to `garin87/gdm_v5` repository
- AI client that supports MCP (Claude Desktop, GitHub Copilot, or Cursor)
- Repository: https://github.com/garin87/gdm_v5

---

## GitHub Issues MCP Server Setup

### Capabilities Enabled

This integration provides three core capabilities:

1. **Search/List Issues** - Find and list issues with filters
2. **Get Issue Details** - Retrieve full issue information including body, labels, assignees
3. **Get Comments/Discussion** - Access issue conversation threads

### Step 1: Choose Your AI Client

#### Option A: Claude Desktop/Code

1. **Locate Claude configuration file:**
   - **Windows:** `%APPDATA%\Claude\claude_desktop_config.json`
   - **macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`
   - **Linux:** `~/.config/Claude/claude_desktop_config.json`

2. **Add GitHub MCP server configuration:**

```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-github"
      ],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "your_github_token_here"
      }
    }
  }
}
```

3. **Create GitHub Personal Access Token:**
   - Go to https://github.com/settings/tokens
   - Click "Generate new token (classic)"
   - Set token name: "Claude MCP Integration"
   - Select scopes (read-only recommended):
     - ? `repo` ? `public_repo` (for public repos)
     - ? `read:org` (if using organization repos)
   - Copy the generated token
   - Replace `your_github_token_here` in the config above

4. **Restart Claude Desktop**

#### Option B: GitHub Copilot

GitHub Copilot has built-in GitHub integration, but you can add explicit MCP support:

1. **Open VS Code Settings**
2. **Navigate to:** Extensions ? GitHub Copilot ? MCP
3. **Add MCP Server** from marketplace
4. **Search for:** "GitHub Issues"
5. **Install and Authenticate** via OAuth

Alternatively, edit MCP configuration directly:

1. **Open Command Palette** (Ctrl+Shift+P / Cmd+Shift+P)
2. **Type:** "GitHub Copilot: Edit MCP Configuration"
3. **Add configuration** similar to Claude setup above

#### Option C: Cursor

1. **Open Cursor Settings** (Ctrl+, / Cmd+,)
2. **Navigate to:** Features ? Tools & MCP
3. **Click "Add Server"**
4. **Select:** GitHub from marketplace
5. **Authenticate** via OAuth or Personal Access Token

Or manually edit `~/.cursor/mcp_config.json`:

```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-github"
      ],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "your_github_token_here"
      }
    }
  }
}
```

---

## Step 2: Verify Installation

### Test Commands

Once configured, test the integration by asking your AI assistant:

#### Test 1: List Issues
```
Show me all open issues in garin87/gdm_v5
```

Expected behavior: AI retrieves and displays current issues

#### Test 2: Get Specific Issue
```
Get details for issue #1 in garin87/gdm_v5
```

Expected behavior: AI shows full issue details with description and metadata

#### Test 3: Get Discussion Context
```
Show me the comments on issue #1 in garin87/gdm_v5
```

Expected behavior: AI displays comment thread from the issue

---

## Step 3: Common Usage Patterns

### During Development

**Finding Related Work:**
```
Are there any open issues related to authentication in this project?
```

**Understanding Context:**
```
What's the background on issue #5? Show me the discussion.
```

**Planning Work:**
```
List all issues labeled "bug" that are assigned to me
```

**Checking Status:**
```
Show me issues in milestone v2.0
```

### Integration with Copilot Workspace

The AI can now reference issues when working on code:

```
I'm working on issue #10. Show me the requirements and generate the service class.
```

```
Based on issue #15, update the authentication flow to match the specification.
```

---

## Step 4: Security Best Practices

### Token Permissions (Read-Only Recommended)

**Minimal Scopes for Read-Only Access:**
- `public_repo` - Access public repositories
- `read:org` - Read organization membership (if needed)

**?? Do NOT enable write scopes unless approved:**
- ? `repo` (full access)
- ? `write:issues`
- ? `delete:repo`

### Token Management

1. **Store securely** - Never commit tokens to repository
2. **Use environment variables** - Keep tokens in config files outside repo
3. **Rotate regularly** - Update tokens every 90 days
4. **Revoke unused tokens** - Clean up old tokens at https://github.com/settings/tokens
5. **Audit access** - Review token usage in GitHub settings

### Client Project Considerations

**?? For client/commercial projects:**

Before enabling MCP integration:
- ? Confirm GitHub API access is approved
- ? Verify AI client (Claude/Copilot) is approved
- ? Use read-only token with minimal scopes
- ? Document integration in security log
- ? Consider using self-hosted proxy if required

**Alternative for strict environments:**
- Use manual copy-paste of issue content
- Export issues to local files for AI context
- Use read-only GitHub API through approved proxy

---

## Step 5: Alternative MCP Integrations

### If Not Using GitHub Issues

#### Linear (Official MCP Server)
```json
{
  "mcpServers": {
    "linear": {
      "command": "npx",
      "args": ["-y", "@linear/mcp-server-linear"],
      "env": {
        "LINEAR_API_KEY": "your_linear_api_key"
      }
    }
  }
}
```

**Capabilities:**
- List Issues
- Get Issue
- Get Comments
- Create/Update Issues (if write access enabled)

**Setup:** https://linear.app/settings/api

#### Jira (Atlassian Rovo MCP Server)
```json
{
  "mcpServers": {
    "jira": {
      "command": "npx",
      "args": ["-y", "@atlassian/mcp-server-rovo"],
      "env": {
        "ATLASSIAN_SITE": "your-site.atlassian.net",
        "ATLASSIAN_API_TOKEN": "your_api_token"
      }
    }
  }
}
```

**Capabilities:**
- Search Issues (JQL)
- Get Issue
- Add/Get Comments
- Transition Issues (if write access enabled)

**Setup:** https://id.atlassian.com/manage-profile/security/api-tokens

#### Asana (Official MCP Server)
```json
{
  "mcpServers": {
    "asana": {
      "command": "npx",
      "args": ["-y", "@asana/mcp-server-asana"],
      "env": {
        "ASANA_ACCESS_TOKEN": "your_asana_token"
      }
    }
  }
}
```

**Capabilities:**
- Find Tasks
- Get Task
- Task Conversations
- Update Tasks (if write access enabled)

**Setup:** https://app.asana.com/0/my-apps

---

## Troubleshooting

### Issue: MCP Server Not Found

**Solution:**
1. Ensure Node.js and npm are installed
2. Run manually: `npx -y @modelcontextprotocol/server-github`
3. Check for error messages
4. Verify internet connectivity

### Issue: Authentication Failed

**Solution:**
1. Verify token is correct and not expired
2. Check token has required scopes
3. Ensure token is for the correct GitHub account
4. Test token manually: `curl -H "Authorization: token YOUR_TOKEN" https://api.github.com/user`

### Issue: Repository Access Denied

**Solution:**
1. Verify token has access to `garin87/gdm_v5`
2. For private repos, ensure `repo` scope is enabled
3. Check if you're a collaborator on the repository

### Issue: AI Not Using MCP Tools

**Solution:**
1. Restart your AI client after configuration
2. Be explicit in prompts: "Use the GitHub MCP server to list issues"
3. Check MCP server logs (usually in AI client logs directory)
4. Verify JSON configuration is valid (no syntax errors)

---

## Usage Examples

### Example 1: Finding Related Issues Before Starting Work

**Prompt:**
```
Before I start working on the authentication bug, show me all open issues 
related to authentication in garin87/gdm_v5. Check if anyone is already 
working on this.
```

**AI Response:**
- Lists all issues with "auth" or "authentication" in title/body
- Shows assignees and status
- Identifies potential duplicates or related work

### Example 2: Understanding Requirements

**Prompt:**
```
I need to implement the feature described in issue #23. Show me the full 
description and all comments so I understand the requirements.
```

**AI Response:**
- Retrieves issue #23 full details
- Shows original description
- Lists all comments with context
- Summarizes key requirements

### Example 3: Code Generation with Context

**Prompt:**
```
Generate a CustomerService class that implements the requirements from 
issue #15. Follow the patterns documented in PROJECT_MAP.md.
```

**AI Response:**
- Reads issue #15 via MCP
- References PROJECT_MAP.md for patterns
- Generates code matching requirements and patterns

### Example 4: Status Updates

**Prompt:**
```
Show me all issues in the v2.0 milestone that are still open. 
Which ones are blocking the release?
```

**AI Response:**
- Lists open issues in milestone
- Identifies dependencies
- Suggests priority order

---

## Configuration for Project Team

### Recommended Team Setup

Create a team-shared configuration template:

**File: `.ai/mcp-setup-template.json`**

```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github"],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "REPLACE_WITH_YOUR_TOKEN"
      }
    }
  }
}
```

**Instructions for team:**
1. Create personal GitHub token with `public_repo` scope
2. Copy template to AI client config location
3. Replace `REPLACE_WITH_YOUR_TOKEN` with personal token
4. Restart AI client

### Audit Logging

If your organization requires audit logs:

**Option 1: GitHub Enterprise**
- Use GitHub Enterprise Server with audit log streaming
- All API calls are logged automatically

**Option 2: Proxy Solution**
- Route MCP calls through authenticated proxy
- Log all requests/responses
- Example: Use NGINX or custom Node.js proxy

**Option 3: Local Logging**
Create a wrapper script that logs all MCP calls:

```javascript
// mcp-github-logged.js
const { spawn } = require('child_process');
const fs = require('fs');

const logFile = './mcp-audit.log';
const timestamp = new Date().toISOString();

// Log the request
fs.appendFileSync(logFile, `${timestamp} - MCP Request Started\n`);

// Spawn the actual MCP server
const child = spawn('npx', ['-y', '@modelcontextprotocol/server-github'], {
  env: process.env,
  stdio: 'inherit'
});

child.on('exit', (code) => {
  fs.appendFileSync(logFile, `${timestamp} - MCP Request Ended (code: ${code})\n`);
});
```

Update config to use wrapper:
```json
{
  "mcpServers": {
    "github": {
      "command": "node",
      "args": ["mcp-github-logged.js"],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "your_token"
      }
    }
  }
}
```

---

## Next Steps

1. **Choose your AI client** (Claude, Copilot, or Cursor)
2. **Create GitHub Personal Access Token** with read-only scope
3. **Add MCP configuration** to your AI client
4. **Restart AI client** to load the integration
5. **Test with a simple query** (e.g., "List open issues in garin87/gdm_v5")
6. **Share setup instructions** with your team

---

## References

- **MCP Specification:** https://modelcontextprotocol.io
- **GitHub MCP Server:** https://github.com/modelcontextprotocol/servers
- **GitHub API Documentation:** https://docs.github.com/en/rest
- **Claude Desktop Configuration:** https://docs.anthropic.com/claude/docs/mcp
- **GitHub Copilot MCP:** https://docs.github.com/en/copilot

---

## Document Maintenance

**Update this file when:**
- MCP server configuration changes
- New AI clients are added to team toolkit
- Security requirements change
- New task tracker integration is needed

**Owner:** Same as PROJECT_MAP.md  
**Last Updated:** [Auto-generated on creation]
