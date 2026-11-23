# Performance Test Strategy

## Objective
Demonstrate modelling and assessing the behavior of a *local synthetic* product search and quote workflow using an API driver and a Chromium browser driver. This is an educational model, not a production capacity certification.

## Risks and assumptions
- Performance may be limited by the test generator (browser CPU/RAM) rather than the target.
- A tiny in-memory dataset makes response times unusually small compared with production.
- Loopback latency differs from network and cloud deployment.
- In-process caches and JIT warmup affect first-run numbers; warmup is intentionally off for easy inspection.
- Running the demo and load generators on the same host causes resource contention.

## Workloads
1. API smoke: one virtual user for five seconds; validate response and data semantics.
2. API steady: two virtual users for thirty seconds; compare median/P95/error percentage.
3. API burst: three scenario arrivals/sec for fifteen seconds; inspect backlog/latency.
4. Browser journey: one browser virtual user for fifteen seconds; track navigation and interaction durations.

## Execution protocol
1. Confirm authorization and target URL (only 127.0.0.1:5055 in code).
2. Verify the local demo application is running and /health reports healthy.
3. Build each project, install Chromium, then run API smoke.
4. Run controlled load profiles separately (no parallel runs on the same host).
5. Run browser scenario with one virtual user.
6. Save report, date/time, machine details, .NET versions, Git SHA, and raw metrics.
7. Repeat at least three times for a meaningful comparison.

## Acceptance guidance (example, not an SLA)
- All journeys should pass business assertions during smoke.
- Any nonzero error rate should be investigated before baselining.
- Set numerical thresholds only after real stable measurements.
- A latency regression should be judged against a known reference environment, not arbitrary Internet figures.

## Investigation examples
- Higher API P95 + high target CPU: investigate server CPU, allocations, contention.
- Higher UI time but stable API: inspect frontend rendering, scripts, assets and browser host saturation.
- Higher failure percentage on burst: inspect connection pool, timeouts, and request queuing.
