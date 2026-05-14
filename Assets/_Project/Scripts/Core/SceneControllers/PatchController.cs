using Cysharp.Threading.Tasks;
using UnityEngine;


namespace SurvivorsLike
{
    public class PatchController : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameManager.Instance.SetGameState(GameState.Patch);

            DelayRun().Forget();
        }

        async UniTask DelayRun()
        {
            // UniTask.Delay(3000)
            // �� 3000 �и���(= 3��) ���� ���

            // cancellationToken:
            // �� "�� �۾��� ����� �� �ִ� ��ȣ"
            // �� �� ���, �� GameObject�� Destroy(����)�Ǹ� �ڵ����� ��ҵ�

            // this.GetCancellationTokenOnDestroy()
            // �� MonoBehaviour�� �ı��� �� �ڵ����� Cancel�Ǵ� ��ū�� ������

            // await:
            // �� Delay�� ���� ������ ���⼭ "�񵿱������� ���"
            // �� Unity ���� �����带 ������ ���� (������ ��� ���ư�)
            await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
            Debug.Log("1�� �� ����");

            await GameManager.Instance.LoadScene("02_Title");
        }
    }
}
