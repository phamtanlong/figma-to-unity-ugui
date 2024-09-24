using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace FastUI {
    public static class Runner {
        private static readonly int[] fibonaci = { 2, 3, 5, 8, 13, 21, 34 };

        private static CoroutineRunner _runner;
        private static readonly Queue<Action> functions = new Queue<Action>();
        private static readonly object lockCall = new object();

        public static CoroutineRunner runner {
            get {
                if (_runner == null || _runner.gameObject == null) {
                    var go = new GameObject("_Runner");
                    _runner = go.AddComponent<CoroutineRunner>();
                    // _runner.StartCoroutine(MainThreadUpdater());
                    Object.DontDestroyOnLoad(_runner);
                }

                return _runner;
            }
        }

        public static Coroutine StartCoroutine(IEnumerator coroutine, MonoBehaviour target = null) {
            if (target != null && target.gameObject.activeInHierarchy)
                return target.StartCoroutine(coroutine);
            return runner.StartCoroutine(coroutine);
        }

        public static void StopCoroutine(Coroutine coroutine, MonoBehaviour target = null) {
            if (coroutine != null) {
                if (target != null && target.gameObject.activeInHierarchy)
                    target.StopCoroutine(coroutine);
                else runner.StopCoroutine(coroutine);
            }
        }

        public static void CallOnMainThread(Action func) {
            if (func == null) throw new Exception("Function can not be null");

            lock (lockCall) {
                functions.Enqueue(func);
            }
        }

        public class CoroutineRunner : MonoBehaviour {
            private void Update() {
                lock (lockCall) {
                    while (functions.Count > 0) {
                        functions.Dequeue().Invoke();
                    }
                }
            }
        }

        #region helpers

        public static Coroutine PostRequest(string url, Dictionary<string, object> data,
            Action<string, UnityWebRequest> callback, MonoBehaviour target = null) {
            UnityWebRequest wr;
            if (data != null) {
                wr = new UnityWebRequest(url, "POST");
                var jsonPayload = JsonConvert.SerializeObject(data);
                var jsonToSend = new UTF8Encoding().GetBytes(jsonPayload.ToCharArray());
                wr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            } else
                wr = UnityWebRequest.Post(url, "");

            wr.downloadHandler = new DownloadHandlerBuffer();
            wr.SetRequestHeader("Content-Type", "application/json");
            return StartCoroutine(WaitRequest(url, wr, callback), target);
        }

        public static Coroutine GetRequest(string url, Action<string, UnityWebRequest> callback,
            MonoBehaviour target = null) {
            var request = UnityWebRequest.Get(NoCache(url));
            return StartCoroutine(WaitRequest(url, request, callback), target);
        }

        private static IEnumerator WaitRequest(string url, UnityWebRequest request,
            Action<string, UnityWebRequest> callback) {
            yield return request.SendWebRequest();
            callback?.Invoke(url, request);
        }

        public static void DownloadTexture(string url, Action<string, Texture2D> callback, int tryCount = 3,
            MonoBehaviour target = null) {
            IEnumerator run() {
                var count = 0;
                while (count < tryCount) {
                    var request = UnityWebRequestTexture.GetTexture(NoCache(url));
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success) {
                        yield return new WaitForSeconds(fibonaci[count]);
                        count++; // fail, retry
                    } else {
                        var myTexture = DownloadHandlerTexture.GetContent(request);
                        callback?.Invoke(url, myTexture); // success
                        yield break;
                    }
                }

                callback?.Invoke(url, null); // fail, return
            }

            StartCoroutine(run(), target);
        }

        public static void DownloadText(string url, Action<string> callback, Dictionary<string, string> headers = null,
            int tryCount = 3, MonoBehaviour target = null) {
            Debug.LogWarning($"Download text: {url}");

            IEnumerator run() {
                var count = 0;
                while (count < tryCount) {
                    var request = UnityWebRequest.Get(NoCache(url));
                    if (headers != null)
                        foreach (var pair in headers)
                            request.SetRequestHeader(pair.Key, pair.Value);
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success) {
                        yield return new WaitForSeconds(fibonaci[count]);
                        count++; // fail, retry
                    } else {
                        callback?.Invoke(request.downloadHandler.text); // success
                        yield break;
                    }
                }

                callback?.Invoke(null); // fail, return
            }

            StartCoroutine(run(), target);
        }

        public static void DownloadFile(string url, Action<byte[]> callback, int tryCount = 3,
            MonoBehaviour target = null) {
            Debug.LogWarning($"Download file: {url}");

            IEnumerator run() {
                var count = 0;
                while (count < tryCount) {
                    var request = UnityWebRequestTexture.GetTexture(NoCache(url));
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success) {
                        yield return new WaitForSeconds(fibonaci[count]);
                        count++; // fail, retry
                    } else {
                        callback?.Invoke(request.downloadHandler.data); // success
                        yield break;
                    }
                }

                callback?.Invoke(null); // fail, return
            }

            StartCoroutine(run(), target);
        }

        private static string NoCache(string url) {
            if (url.Contains("?")) return $"{url}&z={DateTime.UtcNow.ToFileTime()}";

            return $"{url}?z={DateTime.UtcNow.ToFileTime()}";
        }

        public static Coroutine RunAfterFrames(int frame, Action action, MonoBehaviour target = null) {
            IEnumerator WaitToRunActionFrame() {
                if (frame <= 0) {
                    action?.Invoke();
                    yield break;
                }

                for (var i = 0; i < frame; i++) yield return null;
                action?.Invoke();
            }

            return StartCoroutine(WaitToRunActionFrame(), target);
        }

        public static Coroutine RunAfterSeconds(float seconds, Action action, MonoBehaviour target = null) {
            return StartCoroutine(WaitToRunAction(seconds, action), target);
        }

        private static IEnumerator WaitToRunAction(float seconds, Action action) {
            if (seconds > 0)
                yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        public static void RunInterval(float intervalSeconds, int times, Action<int> action,
            MonoBehaviour target = null) {
            for (var i = 0; i < times; i++) {
                var i1 = i;
                StartCoroutine(WaitToRunAction(intervalSeconds * i, () => { action?.Invoke(i1); }), target);
            }
        }

        public static Coroutine RunAfterCheck(Func<bool> ifOk, Action thenRun, MonoBehaviour target = null,
            float timeout = float.MaxValue, Action onTimeout = null) {
            var start = Time.realtimeSinceStartup;

            IEnumerator WaitRunAfterCheck() {
                while (!ifOk()) {
                    if (Time.realtimeSinceStartup - start > timeout) {
                        onTimeout?.Invoke();
                        yield break;
                    }

                    yield return null;
                }

                thenRun?.Invoke();
            }

            return StartCoroutine(WaitRunAfterCheck(), target);
        }

        #endregion
    }
}
