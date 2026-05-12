# Definition of Ready (DoR) Skill

## Purpose
Ensures tasks are fully specified and ready for AI agent execution before work begins. This skill helps validate that all necessary information, context, and constraints are clearly defined.

## Trigger
Use `/dor` or "Check Definition of Ready" when preparing a task for AI agent execution.

## Behavior
When invoked, this skill asks a comprehensive set of questions to ensure the task is well-defined:

### 1. Objective Clarity
- **Q:** What is the primary goal of this task?
- **Q:** What problem does this solve for the user/business?
- **Q:** What is the expected outcome?
- **Q:** How will we know when this task is complete?

### 2. Scope Definition
- **Q:** What is explicitly IN scope for this task?
- **Q:** What is explicitly OUT of scope?
- **Q:** Are there any related tasks that should be done separately?
- **Q:** What parts of the codebase will be affected?

### 3. Technical Context
- **Q:** What files/directories will need to be modified?
- **Q:** Are there any architectural patterns that must be followed?
- **Q:** What existing code/systems does this depend on?
- **Q:** Are there any APIs, libraries, or frameworks involved?

### 4. Constraints
- **Q:** What are the technical constraints? (versions, compatibility, performance)
- **Q:** What are the business constraints? (timeline, budget, compliance)
- **Q:** Are there any security/privacy requirements?
- **Q:** What tools or integrations are restricted?
- **Q:** Are there client-specific restrictions?

### 5. Acceptance Criteria
- **Q:** What are the specific, testable conditions for completion?
- **Q:** What tests need to pass?
- **Q:** What documentation needs to be updated?
- **Q:** Are there any performance benchmarks to meet?

### 6. Context & References
- **Q:** Link to related issue(s)?
- **Q:** Link to project/epic?
- **Q:** Any relevant documentation or design docs?
- **Q:** Are there similar examples in the codebase?
- **Q:** Any discussion threads or decisions to reference?

### 7. Dependencies & Blockers
- **Q:** Are there any blocking issues or PRs?
- **Q:** Do we need input from other team members?
- **Q:** Are there any external dependencies (APIs, services)?
- **Q:** What needs to be completed before starting this?

### 8. Risk Assessment
- **Q:** What could go wrong?
- **Q:** What are the risks of this change?
- **Q:** Is there a rollback plan?
- **Q:** Are there any breaking changes?

### 9. Testing Strategy
- **Q:** What types of tests are needed? (unit, integration, e2e)
- **Q:** What edge cases need to be tested?
- **Q:** Is manual testing required?
- **Q:** What is the testing environment?

### 10. Definition of Done
- **Q:** Code complete and reviewed?
- **Q:** Tests written and passing?
- **Q:** Documentation updated?
- **Q:** Deployed to staging/production?
- **Q:** Stakeholders notified?

## Output Format

The DoR check produces:

### DoR Score: [X/10 categories complete]

#### ? Complete Categories
- List of fully answered categories

#### ?? Incomplete Categories
- List of categories needing more information

#### ?? Blockers
- Critical missing information that prevents starting

### Recommended Next Steps
1. Address blockers
2. Fill in incomplete categories
3. Validate with stakeholders
4. Begin task execution

## Usage Example

```
/dor

Agent: "Let me check the Definition of Ready for this task..."

[Asks questions across all categories]

[Produces DoR Score and recommendations]
```

## Integration with Workflow

1. **Task Creation** ? Run DoR check
2. **DoR Incomplete** ? Gather missing information
3. **DoR Complete** ? Hand to AI agent for execution
4. **During Execution** ? Reference DoR for guidance
5. **Before Completion** ? Verify against acceptance criteria

## Best Practices

### For Task Authors
- Run DoR check early
- Be as specific as possible
- Link to all relevant context
- Define clear acceptance criteria
- Include examples when possible

### For AI Agents
- Reference DoR throughout execution
- Flag ambiguities immediately
- Stay within defined scope
- Validate against acceptance criteria
- Document deviations

### For Reviewers
- Verify DoR was followed
- Check scope adherence
- Validate acceptance criteria met
- Ensure constraints respected

## Red Flags

- ? Vague objectives ("make it better")
- ? Undefined scope ("fix everything")
- ? Missing acceptance criteria
- ? No linked issues/context
- ? Unclear constraints
- ? "We'll figure it out later" approach

## Green Flags

- ? Clear, measurable objective
- ? Explicit scope boundaries
- ? Specific acceptance criteria
- ? All context linked
- ? Constraints documented
- ? Risks identified

## DoR Checklist

Before starting work, ensure:

- [ ] Objective is clear and measurable
- [ ] Scope is explicitly defined (in/out)
- [ ] Technical context is documented
- [ ] Constraints are identified
- [ ] Acceptance criteria are testable
- [ ] All context/issues are linked
- [ ] Dependencies are identified
- [ ] Risks are assessed
- [ ] Testing strategy is defined
- [ ] Definition of Done is clear

**DoR Score Target: 9/10 or higher before starting**

---

**Maintained by:** Project Lead  
**Last Updated:** [Auto-generated]  
**Related:** PROJECT_MAP.md, CLAUDE.md
