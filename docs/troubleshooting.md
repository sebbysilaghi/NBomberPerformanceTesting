# Troubleshooting

- **Connection refused:** Start `dotnet run --project src/Portfolio.DemoApp` in a separate terminal and open `http://127.0.0.1:5055/health`.
- **Port already in use:** Stop the other process on port 5055. This deliberately fixed port keeps tests deterministic.
- **Playwright executable missing:** Build UI project first, then run `pwsh src/Portfolio.UiLoadTests/bin/Debug/net8.0/playwright.ps1 install chromium`.
- **Playwright on Linux:** You might need `install --with-deps chromium` (can require elevated permissions).
- **Build can't restore packages:** Check NuGet connectivity and restore proxy settings.
- **API returns no result:** Check the fixed dataset in `Portfolio.DemoApp/Program.cs`; do not change sample data without updating assertions.
- **Browser scenario hangs:** Ensure the demo server is running, Chromium is installed, and `#status` moves from `Loading…` to product count.
- **Unexpected results:** Avoid comparing browser-load results with API-only latency; check machine utilization.
- **NBomber licensing:** Personal and business contexts have different terms; consult the vendor agreement before client use.
