# SpendWise

A small personal expense tracker built with .NET MAUI (Android + iOS) to show a clean **MVVM** layout
and a cost-aware **GitHub Actions CI/CD** pipeline with a per-run **environment choice** (QA, STAGING or
RELEASE).

## Architecture (MVVM, layered)

```
SpendWise.App (MAUI head)  ──►  SpendWise.Core (net10.0, no MAUI)
 Views (XAML)                    Models        Expense, CategoryTotal
 Services: Shell navigation,     ViewModels    ExpensesViewModel, AddExpenseViewModel
   alerts, BuildInfo             Services      IExpenseRepository (+ JSON / in-memory), IAlertService
 MauiProgram (composition root)  Navigation    INavigationService, Routes
```

- **Model / ViewModel / Service** code lives in `SpendWise.Core`, a plain `net10.0` library, so it builds and
  is unit-tested on any machine with no MAUI workload or emulator.
- **Views** (XAML pages) only bind to view models. They contain no business logic.
- **Platform boundaries** (`Shell` navigation, alerts, file location) are interfaces in `Core` implemented in
  the app head, so view models never touch UI types.
- **DI** is the built-in container, wired in one place (`MauiProgram.cs`): constructor injection only.
- Persistence is a JSON file behind `IExpenseRepository`; swap in SQLite or an API without touching view models.

## Environments

`Build/Environments.props` switches the app title, bundle/package id and a build flag with
`-p:AppEnvironment=QA|STAGING|RELEASE` (default RELEASE). The running app shows its flavour at the bottom of
the main screen.

| Environment | App id | Title |
| --- | --- | --- |
| QA | `com.suryakaran.spendwise.qa` | SpendWise QA |
| STAGING | `com.suryakaran.spendwise.staging` | SpendWise STG |
| RELEASE | `com.suryakaran.spendwise` | SpendWise |

The ids differ so all three install side by side on one device.

## CI/CD

### CI (`.github/workflows/ci.yml`)
- Runs unit tests, then compiles Android; iOS (macOS runner) compiles only after both pass.
- PRs and pushes compile **one environment (QA)** and never use macOS on PRs.
- **Run workflow** lets you pick `QA`, `STAGING`, `RELEASE` or `all`, and whether to include iOS.
- Docs-only changes are skipped.

### CD (`.github/workflows/cd.yml`)
Actions tab → **CD** → **Run workflow**:

| Input | Options |
| --- | --- |
| Environment | `QA`, `STAGING`, `RELEASE`: applies to **both** Android and iOS |
| Platforms | `android`, `ios`, `both` |
| Version | optional display version (default `1.0.<run number>`) |

Only the chosen environment is built, so no minutes go to flavours you did not pick. Tags work too:
`v1.2.3-qa`, `v1.2.3-staging`, `v1.2.3` (RELEASE), all building both platforms.

Cost controls: hosted Linux for tests and Android, macOS only for iOS and only after tests pass, 3-day artifact
retention, job timeouts, Gradle and NuGet caches. Set the repo variables `ANDROID_RUNNER` / `IOS_RUNNER`
(JSON label arrays) to run on your own machines, e.g. `["self-hosted","macOS","ARM64","ios"]`.

### Secrets (all optional)
Add as repository secrets, or on the matching GitHub Environment (`QA`, `STAGING`, `RELEASE`) for
per-environment values. With none set, the pipeline still succeeds and produces an unsigned Android AAB/APK
and a simulator iOS build.

| Secret | Purpose |
| --- | --- |
| `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASSWORD`, `ANDROID_KEY_ALIAS`, `ANDROID_KEY_PASSWORD` | Signed Android AAB + APK |
| `IOS_CERTIFICATE_BASE64`, `IOS_CERTIFICATE_PASSWORD`, `IOS_PROVISIONING_PROFILE_BASE64` | Signed iOS IPA |

Optional repo variable `IOS_TEAM_ID` makes the pipeline verify the profile's team. Encode files with
`base64 -i file | gh secret set NAME`. iOS needs a profile covering the chosen bundle id; a wildcard profile for
`com.suryakaran.spendwise.*` works for all three environments.

## Run locally

```bash
dotnet test SpendWise.Core.Tests                                   # unit tests, no workload needed
dotnet workload install maui-android maui-ios                      # once
dotnet build SpendWise.App -f net10.0-android -p:AppEnvironment=QA
```
