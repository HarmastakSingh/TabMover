// Prototype only. No session values are captured or logged in this alpha.
const HOST = "com.tabmover.bridge";
let port = null;
function connect() {
  try {
    port = chrome.runtime.connectNative(HOST);
    port.onMessage.addListener((message) => {
      if (message?.command === "ping") port?.postMessage({ type: "pong", browser: "chromium", version: "0.3.0" });
    });
    port.onDisconnect.addListener(() => { port = null; });
  } catch {
    // Native host registration is not part of this alpha.
  }
}
chrome.runtime.onStartup.addListener(connect);
chrome.runtime.onInstalled.addListener(connect);
connect();
