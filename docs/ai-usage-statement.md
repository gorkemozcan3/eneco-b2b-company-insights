# AI Usage Statement

This document describes how AI tooling was used while producing this assignment, how the output
was validated, and — just as importantly — which parts of the work were deliberately kept away
from AI.

## Tools used

- **GitHub Copilot (agent mode) in Visual Studio 2026**, used interactively throughout
  implementation, review, and documentation.
- **Predefined agent skills** kept in `.github/skills/` (codebase design, domain
  modelling, and an adversarial "grilling" review skill). These encode a fixed set of standards —
	deep modules, clear boundaries, honest naming, justified abstractions — so that code and design
  reviews were assessed against consistently rather than whatever the model felt like commenting on that day.

## How AI was used

**Design first, generation second.** The working method throughout was to decide the shape of a
thing myself and then delegate the typing:

- **Models and contracts** — I designed the external contracts and the public DTOs, including
  their names and properties and the mapping between them, then had AI write out the record
  declarations.
- **Service interfaces** — I defined `ICompanyService`, `IKvkFinderApiClient`, and
  `ICompetitorPricingApiClient` (the application boundaries), then had AI produce the
  implementations behind them.
- **Component implementation** — most component bodies were AI-generated from those designs, and
  every one was read, adjusted, and accepted only after I was satisfied it was correct and
  idiomatic. Several were rewritten or simplified before being kept.
- **Test exploration and implementation** — I identified which boundaries were worth testing and which
  behaviours mattered (mapping rules, error contract, malformed external responses, resilience,
  information leakage); AI helped enumerate edge cases and write the test bodies.
- **Architectural interrogation** — AI was used as a teammate to challenge decisions and
  surface future failure modes: where an abstraction was disproportionate, which exception mapping
  would break once cancellation was introduced, whether an external 404 should really be a 404.
  Several decisions in the design document exist because that questioning exposed a weakness.
- **Codebase assessment against the assignment** — I ran review passes over the finished code,
  driven by the skills above and an explicit checklist (SOLID, REST and Microsoft/community API
  conventions, separation of concerns, HTTP and status-code semantics, error handling
  consistency, security exposure, versioning, observability, rate limiting, caching, timeouts,
  retries, concurrency, and over-engineering). This produced a prioritised list of real gaps —
  notably end-to-end `CancellationToken` propagation annd mapping correct error semantics — which
  I then implemented and verified.
- **Documentation** — this statement, the `README`, and the backend design document were drafted
  with AI assistance from my own notes, decisions, and trade-off reasoning, then edited.

## What AI was deliberately *not* used for

Some decisions require human reasoning about a domain, a product, and an organisation, and
delegating them would have produced reasonable but unowned choices. These were made
independently, and AI was only asked to implement or critique them afterwards:

- **Contract ambiguity.** The external pricing contract distinguishes electricity and gas units,
  while the required public contract does not. The current electricity-only interpretation is
  treated as a provisional assumption requiring product clarification, not as an AI-derived fact.
- **Error semantics.** Which failures are the caller's fault, which belong to an external API, and
  which are ours: 502 for external failures, 500 for unexpected internal failures, and the open
  question of 404 versus an empty 200 for pricing.
- **Scope and restraint.** I chose a single project and left persistence and caching out of the
  demo. Authentication, traffic controls, caching, and telemetry remain production considerations
  whose implementation depends on actual requirements. Judging for a two-endpoint service is
  exactly the kind of decision where AI tends to add too much.

## How AI output was validated

- **Read before accepted.** No generated file was committed unreviewed. Output that was verbose,
  speculative, or over-abstracted was cut or rewritten.
- **Automated verification.** The solution builds clean and unit/integration tests cover generated
	behaviour such as contract mapping, the provisional electricity filter, malformed and empty
  external responses, exception-to-status-code mapping, retry behaviour on a transient external
  failure, and a check that exception messages never leak into a `ProblemDetails` response.
- **Static analysis.** `SonarAnalyzer.CSharp` runs as part of the build, with nullable reference
  types and documentation generation enabled.
- **Cross-checking against sources.** Framework guidance (Problem Details, exception handling,
  HTTP resilience, and telemetry) was verified against official Microsoft documentation rather
  than accepted from generated text.

## Summary

AI accelerated the mechanical work — writing records, filling in implementations behind
interfaces I had designed, enumerating test cases, and drafting plain text from my notes — and served
as a reviewer that argues back. The architecture, validation policy, contract assumptions, error
semantics, and decisions about what *not* to build were mine. Everything the tooling produced had
to survive review, tests, and human inspection before it stayed in the repository.
