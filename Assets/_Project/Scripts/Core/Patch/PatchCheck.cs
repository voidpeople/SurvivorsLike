using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

/*
 JSON 메세지
 {
  "status": "ok",
  "message": "정상",
  "requiredVersion": "1.2.0",
  "latestVersion": "1.4.0"
}
 */

[System.Serializable]
public class PatchResponse
{
    public string status;
    public string message;         
    public string requiredVersion; //최소 요구 버전
    public string latestVersion;   //최신 패치 버전
}

public enum PatchCheckStatus : byte
{
    UpToDate,          //최신 상태
    NeedPatch,         //패치 필요
    ForcePatch,        //강제 패치
    NetworkError,      //서버 통신 실패
    ServerMaintenance, //서버 점검
    InvalidResponse    //알수 없는 오류
}

public class PatchCheckResult
{
    public PatchCheckStatus Status;
    public string Message;
    public string RequiredVersion;
    public string LatestVersion;
}

//패치 서버와 통신하여 패치를 할 지를 검사한다.
public static  class PatchCheck
{
    public static async UniTask<PatchCheckResult> CheckPatchAsync()
    {
        //서버 URL
        string patchServerURL = "https://myPatchServer.com/getVersion";

        //using 명령은 try finally 명령와 같다.
        using (UnityWebRequest request = UnityWebRequest.Get(patchServerURL))
        {
            //서버에 요청을 한다.
            await request.SendWebRequest();

            //통신 실패 처리~
            if(request.result != UnityWebRequest.Result.Success)
            {
                UnityEngine.Debug.LogWarning("패치 서버 통신 실패~");
                return new PatchCheckResult
                {
                    Status = PatchCheckStatus.NetworkError,
                    Message = "네트워크 오류!"
                };
            }
            
            string jsonData = request.downloadHandler.text;

            //서버로 부터 응답 받은 데이터 파싱~
            PatchResponse response;
            try
            {
                response = JsonConvert.DeserializeObject<PatchResponse>(jsonData);
            }
            catch
            {
                return new PatchCheckResult
                {
                    Status = PatchCheckStatus.InvalidResponse,
                    Message = "서버 데이터 파싱 실패!"
                };
            }

            //서버 데이터 상태 확인
            if (response == null || string.IsNullOrEmpty(response.status))
            {
                return new PatchCheckResult
                {
                    Status = PatchCheckStatus.InvalidResponse,
                    Message = "서버 데이터 오류!"
                };
            }


            //서버 상태 분기
            switch (response.status)
            {
                //서버 정상~
                case "ok":
                    break;

                //서버 점검중~
                case "maintenance":
                    return new PatchCheckResult
                    {
                        Status = PatchCheckStatus.ServerMaintenance,
                        Message = response.message
                    };

                default:
                    return new PatchCheckResult
                    {
                        Status = PatchCheckStatus.InvalidResponse,
                        Message = "알 수 없는 상태"
                    };
            }

            string localPatchedVerion = UnityEngine.Application.version;

            //로컬 패치된 버전이 최소 패치 버전 보다 작을 경우 강제 업데이트 시켜야 한다.
            bool isForceUpdate = CompareVersion(localPatchedVerion, response.requiredVersion) < 0;
            if (isForceUpdate == true)
            {
                return new PatchCheckResult
                {
                    Status = PatchCheckStatus.ForcePatch,
                    Message = response.message,
                    RequiredVersion = response.requiredVersion,
                    LatestVersion = response.latestVersion
                };
            }

            //localPatchedVerion 버전이 response.latestVersion 버전보다 작으면
            //음수 값을 반환하므로 isNeedPatch은 true가 된다.
            bool isNeedPatch = CompareVersion(localPatchedVerion, response.latestVersion) < 0;
            if (isNeedPatch == true)
            {
                return new PatchCheckResult
                {
                    Status = PatchCheckStatus.NeedPatch,
                    Message = response.message,
                    RequiredVersion = response.requiredVersion,
                    LatestVersion = response.latestVersion
                };
            }

            return new PatchCheckResult
            {
                Status = PatchCheckStatus.UpToDate,
                Message = "최신 버전",
                RequiredVersion = response.requiredVersion,
                LatestVersion = response.latestVersion
            };
        }
    }

    //"1.10.0"와 "1.2.0"을 제대로 비교 하려면 문자열을 int형으로 변환하여 비교 해야 함~
    //매개변수로 입력되는 a버전이 b버전 보다 작으면 -1을 반환한다.
    private static int CompareVersion(string aVer, string bVer)
    {
        var aVerParts = aVer.Split('.');
        var bVerParts = bVer.Split('.');

        int length = Mathf.Max(aVerParts.Length, bVerParts.Length);

        for (int i = 0; i < length; i++)
        {
            int aVerNum = i < aVerParts.Length ? int.Parse(aVerParts[i]) : 0;
            int bVerNum = i < bVerParts.Length ? int.Parse(bVerParts[i]) : 0;

            if (aVerNum != bVerNum)
            {
                //aVerNum이 bVerNum 보다 값이 작으면 -1을 반환한다.
                return aVerNum.CompareTo(bVerNum);
            }
        }

        return 0;
    }

    private static string GetLocalPatchedVersion()
    {
        return UnityEngine.Application.version;
    }
}
