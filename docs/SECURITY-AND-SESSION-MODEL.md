# Security and session model

Cookies, origin storage, and form state are credentials or sensitive user data.

Required before session transfer ships:
1. Explicit, preferably optional, host permissions.
2. Native messaging only; no unauthenticated localhost HTTP service.
3. Memory-only payload handling; never write session data to logs, crash reports, telemetry, config, or temp files.
4. Clear transient data after transfer where practical.
5. Exclude private/incognito windows unless explicitly enabled.
6. Never read or migrate browser password-manager databases, passkeys, or authenticator material.
7. Close source only after destination restoration returns a successful verification result.
8. Failed or ambiguous transfer leaves source untouched.

Cookie migration is best effort. Partition keys, SameSite, secure-context rules, host permissions, device binding, and server-side policies can prevent restoration or invalidate sessions.

Form restoration is opt-in. Never capture password fields, payment fields, security answers, or file inputs.
