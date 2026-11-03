# Test plan — v0.3.0-alpha

This alpha is not ready for functional tab-transfer testing.

## Desktop shell
- Launches without terminal.
- Settings persist across restart.
- Tray open/exit works.
- Installer and portable package launch correctly.

## Browser integration
- Native host registration correct per browser.
- Permission prompts explicit.
- Browser connection health visible.
- No session data written to logs or disk.

## Transfer safety (future)
- Destination failure never closes source.
- Cookie restoration failure never closes source.
- Keep-original setting is respected.
- Close-after-success only closes after verification.
- Private windows excluded by default.

## Compatibility matrix (future)
Brave ↔ Opera; Chrome ↔ Edge; Firefox ↔ Chromium; same-browser different-window control case.
