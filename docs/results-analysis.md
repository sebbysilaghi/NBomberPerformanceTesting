# Results analysis — 23 November 2026

## Scope and source evidence

Four original NBomber HTML reports are stored in [`reports/`](../reports/). The numbers below were extracted from their embedded `FinalStats` and scenario `StepStats` objects; they are **observed measurements**, not invented targets or estimates. Test sessions were run on a single Windows/.NET 8 machine against a loopback demo application. The reports identify NBomber version **6.7.1**.

### Executive summary

| Workload | Duration / model | Completed journeys | Successful measured steps | Step throughput | Overall step P50 | Overall step P95 | Overall step P99 | Max step | Failed steps |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| [Api Smoke](../reports/api-smoke-2026-11-23.html) | 1 user, 5 s | 1,003 | 2,006 | 401.20/s | 1.60 ms | 2.86 ms | 4.60 ms | 788.75 ms | 0 |
| [Api Steady](../reports/api-steady-2026-11-23.html) | 2 users, 30 s | 9,850 | 19,700 | 656.67/s | 2.88 ms | 4.38 ms | 5.53 ms | 349.59 ms | 0 |
| [Api Burst](../reports/api-burst-2026-11-23.html) | 3 arrivals/s, 15 s | 45 | 90 | 6.00/s | 2.75 ms | 20.22 ms | 47.90 ms | 270.30 ms | 0 |
| [Ui Browser](../reports/ui-browser-2026-11-23.html) | 1 user, 15 s | 45 | 90 | 6.00/s | 105.02 ms | 141.70 ms | 148.99 ms | 667.48 ms | 0 |

**Counting note:** Each journey contains two measured steps. NBomber reports the scenario-wide statistics here for the pooled steps, not for the summed end-to-end journey latency. Throughput in this table is step throughput, not business-journey throughput. For an approximate completed-journey rate in these runs, halve the displayed step rate.

## Per-step latency evidence

| Workload | Measured step | Count | Mean | P95 | P99 |
| --- | --- | ---: | ---: | ---: | ---: |
| Api Smoke | `GET_products` | 1,003 | 2.34 ms | 2.36 ms | 4.32 ms |
| Api Smoke | `POST_quote` | 1,003 | 2.05 ms | 3.00 ms | 4.61 ms |
| Api Steady | `GET_products` | 9,850 | 2.98 ms | 4.33 ms | 5.51 ms |
| Api Steady | `POST_quote` | 9,850 | 3.00 ms | 4.42 ms | 5.56 ms |
| Api Burst | `GET_products` | 45 | 10.49 ms | 23.33 ms | 270.33 ms |
| Api Burst | `POST_quote` | 45 | 4.98 ms | 16.78 ms | 41.92 ms |
| Ui Browser | `load_home_page` | 45 | 134.01 ms | 146.30 ms | 667.65 ms |
| Ui Browser | `search_mouse` | 45 | 47.66 ms | 64.70 ms | 105.02 ms |

## Interpretation

### 1. API smoke: initial connectivity and correctness
The smoke workload completed **1,003 two-step journeys** in 5 seconds, with **zero recorded failed steps**. A pooled step P95 of **2.86 ms** suggests fast local processing for the sampled requests. A maximum measured step of **788.75 ms** shows that even this light run had an occasional long latency observation. The test is too short for capacity conclusions.

### 2. API steady: sustained bounded concurrency
At **two concurrent virtual users**, the workload completed **9,850 journeys** over 30 seconds without recorded failed steps. P95 was **4.38 ms** and P99 **5.53 ms** across the pooled steps. The throughput was **656.67 measured steps/sec** (about **328.33 journeys/sec**). This is an empirical result for a trivial in-memory target, not a projection for a production service.

### 3. API burst: fixed-arrival workload
The arrival model scheduled **three new journeys each second** for 15 seconds, completing **45 journeys** (**90 steps**) without recorded failures. P95 increased to **20.22 ms** (versus **4.38 ms** in the steady run), but this does **not** establish that higher load caused the difference: the burst test actually had a *lower arrival rate* than the measured steady-state throughput, used a different model, and had a much smaller sample. Possible influences include runtime scheduling, cold-start effects, test-host interference, or outliers. A controlled repeated experiment is required.

### 4. UI browser: real Chromium interaction
The one-user, 15-second browser run completed **45 navigation-and-search journeys** (**90 measured steps**) without recorded failures. P95 across measured steps was **141.70 ms**. The navigation step P95 was **146.30 ms** and the search step P95 was **64.70 ms**. Navigation also had an observed maximum of **667.48 ms**. These timings include browser and automation overhead; they are neither raw API latency nor Web Vitals such as LCP or INP.

## Findings and technical limitations

1. **No step failures recorded.** All four reports show 0 failures, and the test source validates expected response data. This confirms only the covered checks for these runs.
2. **No measured latency SLA.** The HTML metadata contains no configured NBomber thresholds. Accordingly, do not label these tests a formal SLO pass.
3. **Incomplete telemetry in historical reports.** NBomber emitted hints about not recording status codes and transfer sizes. Their zero-byte fields represent *unmeasured values*, not literally empty HTTP bodies. An enhancement could add `Response.Ok(statusCode: ...)` and `Response.Ok(sizeBytes: ...)` where appropriate, followed by fresh measurements.
4. **Single local node and synthetic data.** The server and load generator are on the same machine and communicate over localhost. CPU saturation, memory pressure, distributed latency, real data volumes and shared infrastructure were not evaluated.
5. **Percentile comparability.** These are pooled step statistics; GET, POST, navigation, and search measure different work. Compare the same step across matching runs rather than combining them into a production claim.
6. **Sample size and warm-up.** The smoke and burst samples are especially small, and the original code disables warm-up. Repetition, warm-up, and host telemetry would strengthen inference.

## Recommended next experiment

1. Preserve these HTML files as **baseline run evidence** for the original code; changes to instrumentation will need **new** reports.
2. Repeat the *same workload model* at fixed parameters at least 3 times. Record machine load and application runtime configuration.
3. Capture status codes, measured transfer size, and host CPU/memory.
4. Add a controlled concurrent-user sweep (1, 2, 4, 8) while monitoring P95/P99 and errors; stop if host resources become constrained.
5. Only then define credible P95/error-rate performance acceptance criteria for this local scenario.

## Provenance

The report files were supplied by the repository author. Their embedded metadata identifies NBomber 6.7.1 on Windows with .NET 8; precise host characteristics and competing background workloads were not independently verified. The source code included here preserves the tested scenario behavior and strips explanatory comments only. Nothing in this analysis claims that tests were executed by this packaging process.
