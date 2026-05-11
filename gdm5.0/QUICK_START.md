# AI Tooling Quick Start Guide

## ?? Your Repository is Now AI-Ready!

This repository has been equipped with comprehensive AI-assisted development support through the **Cartograph skill** and related documentation.

---

## ?? What Was Created

### 1. **PROJECT_MAP.md** (Root Directory)
**The single source of truth for your project architecture**

- Complete project structure
- Domain models and relationships
- Technology stack
- Architectural patterns
- Development workflows

**?? Start here** when you need to understand the project architecture.

### 2. **Claude Code Integration**

#### `.claude/CLAUDE.md`
Always-on project guidance that Claude Code automatically references.

#### `.claude/skills/Cartograph/SKILL.md`
Repository mapping skill that provides automatic context to Claude Code.

**How it works:** When you use Claude Code, it automatically reads these files and understands your project structure, patterns, and conventions.

### 3. **GitHub Copilot Configuration**

#### `.github/copilot-instructions.md`
Specific instructions for GitHub Copilot with code examples and patterns.

**How it works:** GitHub Copilot reads this file and generates code that follows your established patterns.

### 4. **Universal AI Support**

#### `.ai/README.md`
Configuration reference for all AI tools (Cursor, Tabnine, etc.)

### 5. **MCP Integration (Task Tracker)**

#### `.ai/MCP_QUICK_START.md`
Quick setup guide to connect GitHub Issues (or other task trackers) through MCP.

#### `docs/MCP_INTEGRATION_GUIDE.md`
Comprehensive guide for setting up task tracker integration.

**How it works:** Once configured, your AI assistant can retrieve issue information, comments, and discussions without leaving your IDE.

---

## ?? How to Use

### For Claude Code Users

Simply ask questions naturally:
- "What is the architecture of this project?"
- "How do I add a new service?"
- "Where are the product models?"
- "Show me the pattern for creating a new controller"

The Cartograph skill will automatically provide context!

### For GitHub Copilot Users

Start typing and Copilot will:
- Suggest code following your patterns
- Use your naming conventions
- Generate services, controllers, and DTOs in your established style

Example:
```csharp
// Type: "Create a new ProductService"
// Copilot will suggest code following your Service Layer pattern
```

### For Other AI Tools

1. Read **PROJECT_MAP.md** before starting
2. Reference **.ai/README.md** for guidance
3. Follow documented patterns

### For Task Tracker Integration (MCP)

**Quick setup (5 minutes):**
1. Read **`.ai/MCP_QUICK_START.md`**
2. Create GitHub Personal Access Token
3. Configure your AI client
4. Ask: "List open issues in garin87/gdm_v5"

**Benefits:**
- ?? Find related issues before coding
- ?? Get requirements without switching apps
- ?? Read discussion context
- ?? Plan work based on issues
- ?? Generate code with issue context

---

## ?? Quick Examples

### Ask Claude Code:
```
"How do I create a new order service?"
```

Claude will reference the Cartograph skill and provide:
- The file locations to create
- The patterns to follow
- Example code following your conventions

### Use GitHub Copilot:
```csharp
// Start typing in a new file:
public interface ICustomerService

// Copilot will suggest:
public interface ICustomerService
{
    Task<ApplicationResponseGeneric<CustomerDTO>> GetCustomerAsync(int id);
    Task<ApplicationResponseGeneric<List<CustomerDTO>>> GetAllAsync();
    // ... following your established patterns
}
```

---

## ? Next Steps

### 1. Assign Document Owner
Open **PROJECT_MAP.md** and edit line 3:
```markdown
**Owner:** [YOUR NAME]
```

### 2. Review Documentation
- Read through `PROJECT_MAP.md` 
- Verify it matches your current architecture
- Add any missing information

### 3. Set Up MCP Integration (Optional but Recommended)
- Follow **`.ai/MCP_QUICK_START.md`** (5 minutes)
- Connect GitHub Issues to your AI client
- Test: Ask "List issues in garin87/gdm_v5"

### 4. Commit Everything
```bash
git add PROJECT_MAP.md .claude/ .ai/ .github/ docs/ *.md
git commit -m "Add Cartograph skill, AI tooling, and MCP integration"
git push origin AI-tooling-setup
```

### 5. Test It Out!

#### Test with Claude Code:
1. Open your IDE with Claude Code
2. Ask: "What are the main domain areas in this project?"
3. Watch it reference the Cartograph skill!

#### Test with GitHub Copilot:
1. Create a new service file
2. Start typing a service interface
3. Watch Copilot suggest code following your patterns!

---

## ?? Maintenance

### Update Documentation When:
- ?? Adding new major features
- ?? Changing architectural patterns
- ?? Upgrading technology stack
- ?? Modifying development workflows

### Who Updates:
The **Document Owner** (assigned in PROJECT_MAP.md) is responsible for keeping documentation current.

### How to Update:
1. Edit **PROJECT_MAP.md** for architectural changes
2. Update **CLAUDE.md** if workflows change
3. Sync **Cartograph/SKILL.md** with PROJECT_MAP changes
4. Commit and push changes

---

## ?? Key Concepts

### Single Source of Truth
**PROJECT_MAP.md** is the authoritative documentation. All other files reference it.

### Always-On Guidance
Claude Code automatically loads context from `.claude/` files. You don't need to do anything special!

### Pattern Consistency
AI tools use these configurations to generate code that matches your established patterns.

### Team Alignment
Everyone (humans and AI) works from the same architectural understanding.

---

## ?? Pro Tips

### For Best Results:

1. **Be Specific in Prompts**
   - ? "Create a service"
   - ? "Create a CustomerService following the established Service Layer pattern"

2. **Reference Documentation**
   - ? "How does authentication work?"
   - ? "Based on the project map, how does JWT authentication work in this project?"

3. **Keep Documentation Updated**
   - Update PROJECT_MAP.md when making architectural changes
   - Include doc updates in pull requests

4. **Use Explicit Skill References**
   - Ask Claude: "Use the Cartograph skill to explain the order domain"

---

## ?? Troubleshooting

### Claude Code not providing good context?
- ? Verify `.claude/skills/Cartograph/SKILL.md` exists
- ? Try explicitly asking: "Use the Cartograph skill"
- ? Check that PROJECT_MAP.md is accurate

### GitHub Copilot generating incorrect patterns?
- ? Ensure `.github/copilot-instructions.md` is committed
- ? Be more explicit in your code comments
- ? Review and update the instructions file

### Documentation seems outdated?
- ? Notify the Document Owner
- ? Create a pull request with updates
- ? Schedule a documentation review session

---

## ?? File Locations Reference

```
Your Repository Root/
?
??? PROJECT_MAP.md                    ?? Start here!
??? AI_SETUP_VERIFICATION.md          ?? Detailed verification checklist
??? QUICK_START.md                    ?? You are here!
?
??? .claude/
?   ??? CLAUDE.md                     ?? Claude Code guidance
?   ??? skills/
?       ??? Cartograph/
?           ??? SKILL.md              ?? Auto-context skill
?
??? .ai/
?   ??? README.md                     ?? Universal AI reference
?
??? .github/
    ??? copilot-instructions.md       ?? Copilot configuration
```

---

## ?? Success!

Your repository now has:

? Comprehensive architectural documentation  
? Claude Code Cartograph skill integration  
? GitHub Copilot configuration  
? Universal AI tool support  
? Always-on project guidance  
? Pattern consistency enforcement  

**You're ready to leverage AI-assisted development with full project context!**

---

## ?? Questions?

- **Architecture Questions:** See PROJECT_MAP.md
- **Setup Issues:** See AI_SETUP_VERIFICATION.md
- **Daily Usage:** This guide (QUICK_START.md)
- **Detailed How-To:** See .claude/CLAUDE.md

---

**Happy Coding with AI! ??**

---

**Repository:** https://github.com/garin87/gdm_v5  
**Branch:** AI-tooling-setup  
**Created:** [Auto-generated]
