// Prototype only. No session values are captured or logged in this alpha.
try {
  const port = browser.runtime.connectNative("com.tabmover.bridge");
  port.onMessage.addListener((message) => {
    if (message?.command === "ping") port.postMessage({ type: "pong", browser: "firefox", version: "0.3.0" });
  });
} catch {
  // Native host registration is not part of this alpha.
}
