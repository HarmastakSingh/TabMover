# Architecture — v0.3.0-alpha

## Components
- Desktop shell: Windows settings, tray, notifications, visual overlay, diagnostics.
- Drag coordinator: pointer observation, source/target detection, drag state machine.
- Browser adapters: Chromium and Firefox WebExtensions.
- Native messaging bridge: local IPC between browser adapters and desktop coordinator.
- Transfer coordinator: capture, validate, restore, verify, and conditionally close source.
- Installer: Inno Setup package plus portable ZIP.

## Planned protocol
Use browser native messaging and a local authenticated IPC channel. Do not expose an unauthenticated localhost HTTP listener. Session payloads are ephemeral and memory-only. Protocol messages must be versioned and bounded.

## Adapter responsibilities
Identify selected tab; read URL/title; request cookie access after host permission; capture current-origin localStorage/sessionStorage where allowed; capture scroll position and optionally safe form values; create destination tab; restore state after navigation; return structured verification results.

## Constraints
Cookie migration may fail because of partition keys, SameSite, Secure, host permissions, browser-specific policies, device binding, or server-side invalidation. sessionStorage is scoped to a tab and origin. A live JavaScript runtime cannot be cloned through ordinary extension APIs.

## Current implementation status
The desktop settings shell, JSON configuration persistence, integration manifests/prototypes, installer script, and CI packaging definition are present. Native broker, drag coordinator, and end-to-end state transfer are not implemented in this alpha.
