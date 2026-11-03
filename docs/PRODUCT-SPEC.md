# Product specification — v0.3.0-alpha

## Promise
A single Windows application provides a visible drag-and-drop experience for transferring tabs between browsers. Background integrations expose browser APIs. The desktop application remains the primary control surface.

## User settings
- Close source after verified transfer: configurable, default ON.
- Keep source tab open: configurable alternative.
- Any failure or uncertain verification: always keep source open.
- Destination placement: next to active tab / end / browser default.
- Notifications and visual drop overlay: configurable.
- Private/incognito windows: disabled by default.
- Session components: cookies, localStorage, sessionStorage, scroll position, optional safe form values.

## Transfer lifecycle
1. Detect drag and identify source browser/tab.
2. Capture supported state through source integration.
3. Show destination affordance.
4. Identify target browser and insertion position.
5. Restore URL and supported state in destination.
6. Verify destination load and restoration signals.
7. Close source only if configured and verification succeeds.
8. Clear transient payload from memory.

## Safety invariants
- Never close source when destination creation/restoration fails.
- Never log session values.
- Never persist session payloads to disk.
- Never migrate password vaults or passkeys.
- Treat verification as partial: page load does not prove authenticated state.
