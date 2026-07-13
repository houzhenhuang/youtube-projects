const tokenInput = document.getElementById("tokenInput");
const noteContentInput = document.getElementById("noteContentInput");
const resultSection = document.getElementById("resultSection");
const resultEndpoint = document.getElementById("resultEndpoint");
const resultStatus = document.getElementById("resultStatus");
const resultTime = document.getElementById("resultTime");
const resultHint = document.getElementById("resultHint");
const resultOutput = document.getElementById("resultOutput");

const tokenStorageKey = "tenant-notes-demo-token";
const apiBaseUrl = window.location.protocol === "file:"
    ? "https://localhost:7192"
    : window.location.origin;

initialize();

document.getElementById("saveTokenButton").addEventListener("click", () => {
    localStorage.setItem(tokenStorageKey, tokenInput.value.trim());
    renderResult({
        endpoint: "localStorage",
        ok: true,
        status: "已保存",
        title: "JWT Token 已保存到浏览器本地存储",
        payload: {
            hasToken: tokenInput.value.trim().length > 0
        }
    });
});

document.getElementById("meButton").addEventListener("click", () => {
    invokeApi("GET", "/me");
});

document.getElementById("notesButton").addEventListener("click", () => {
    invokeApi("GET", "/notes");
});

document.getElementById("createNoteButton").addEventListener("click", () => {
    const content = noteContentInput.value.trim();

    if (!content) {
        renderResult({
            endpoint: "POST /notes",
            ok: false,
            status: "前端校验",
            title: "创建笔记失败",
            payload: {
                error: "笔记内容不能为空"
            }
        });
        return;
    }

    invokeApi("POST", "/notes", { content });
});

document.getElementById("clearNoteButton").addEventListener("click", () => {
    noteContentInput.value = "";
    noteContentInput.focus();
});

document.getElementById("clearResultButton").addEventListener("click", () => {
    renderIdleState();
});

function initialize() {
    const savedToken = localStorage.getItem(tokenStorageKey);
    if (savedToken) {
        tokenInput.value = savedToken;
    }

    renderIdleState();
}

async function invokeApi(method, url, body) {
    const token = tokenInput.value.trim();
    const requestUrl = buildApiUrl(url);

    if (!token) {
        renderResult({
            endpoint: `${method} ${url}`,
            ok: false,
            status: "缺少 Token",
            title: "请求未发送",
            payload: {
                error: "请先在认证区域输入 JWT Token"
            }
        });
        tokenInput.focus();
        return;
    }

    setPendingState(method, url, requestUrl);

    try {
        const response = await fetch(requestUrl, {
            method,
            headers: buildHeaders(Boolean(body)),
            body: body ? JSON.stringify(body) : undefined
        });

        const text = await response.text();
        const parsed = tryParseJson(text);

        renderResult({
            endpoint: `${method} ${url}`,
            ok: response.ok,
            status: `${response.status} ${response.statusText}`.trim(),
            title: response.ok ? "调用成功" : "调用失败",
            payload: parsed ?? text ?? null
        });

        if (response.ok && method === "POST" && url === "/notes") {
            noteContentInput.value = "";
        }
    } catch (error) {
        renderResult({
            endpoint: `${method} ${url}`,
            ok: false,
            status: "网络异常",
            title: "请求发送失败",
            payload: {
                error: error instanceof Error ? error.message : String(error)
            }
        });
    }
}

function buildApiUrl(path) {
    return new URL(path, `${apiBaseUrl}/`).toString();
}

function buildHeaders(hasJsonBody) {
    const headers = {
        Accept: "application/json",
        Authorization: `Bearer ${tokenInput.value.trim()}`
    };

    if (hasJsonBody) {
        headers["Content-Type"] = "application/json";
    }

    return headers;
}

function tryParseJson(text) {
    if (!text) {
        return null;
    }

    try {
        return JSON.parse(text);
    } catch {
        return text;
    }
}

function setPendingState(method, url, requestUrl) {
    revealResultArea();
    resultEndpoint.textContent = `${method} ${url}`;
    resultStatus.textContent = "请求中...";
    resultStatus.className = "mt-2 inline-flex rounded-full bg-amber-100 px-3 py-1 text-sm font-semibold text-amber-900";
    resultTime.textContent = formatNow();
    resultHint.textContent = "正在等待接口响应";
    resultOutput.textContent = JSON.stringify(
        {
            message: "请求已发送",
            endpoint: `${method} ${url}`,
            requestUrl
        },
        null,
        2
    );
}

function renderIdleState() {
    resultEndpoint.textContent = "尚未调用";
    resultStatus.textContent = "等待操作";
    resultStatus.className = "mt-2 inline-flex rounded-full bg-slate-200 px-3 py-1 text-sm font-semibold text-slate-700";
    resultTime.textContent = "--";
    resultHint.textContent = "调用后自动刷新";
    resultOutput.textContent = JSON.stringify(
        {
            message: "等待发起 API 调用"
        },
        null,
        2
    );
}

function renderResult({ endpoint, ok, status, title, payload }) {
    revealResultArea();
    resultEndpoint.textContent = endpoint;
    resultStatus.textContent = status;
    resultStatus.className = ok
        ? "mt-2 inline-flex rounded-full bg-emerald-100 px-3 py-1 text-sm font-semibold text-emerald-900"
        : "mt-2 inline-flex rounded-full bg-rose-100 px-3 py-1 text-sm font-semibold text-rose-900";
    resultTime.textContent = formatNow();
    resultHint.textContent = title;
    resultOutput.textContent = JSON.stringify(
        {
            title,
            endpoint,
            status,
            apiBaseUrl,
            payload
        },
        null,
        2
    );
}

function revealResultArea() {
    if (!resultSection) {
        return;
    }

    resultSection.scrollIntoView({
        behavior: "smooth",
        block: "start"
    });

    resultSection.classList.add("ring-4", "ring-sky-200");
    window.clearTimeout(revealResultArea.highlightTimer);
    revealResultArea.highlightTimer = window.setTimeout(() => {
        resultSection.classList.remove("ring-4", "ring-sky-200");
    }, 900);
}

function formatNow() {
    return new Date().toLocaleString("zh-CN", {
        hour12: false
    });
}
