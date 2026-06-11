using UnityEngine;



namespace SurvivorsLike
{
    //이 싱글톤 클래스를 상속받는 클래스는 씬의 게임 오브젝트에 해당 클래스 스크립트를 
    //어태치 하여 씬이 로드시 자연스럽게 싱글톤 클래스의 Awake()함수가 호출되어
    //클래스의 인스턴스가 초기화 되게 해 주어야 한다.
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        //프로그램이 종료중 인지?
        private static bool _applicationQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationQuitting == true)
                    return null;

                if (_instance == null)
                {
                    //이미 실행중인 객체가 있다면 찾아서 설정~
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError($"[Singleton] {typeof(T).Name}가 씬에 존재하지 않음~ 첫 씬에 이 클래스 스크립트가 어태치 된 오브젝트를 배치하세요.");
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            //프로그램이 종료 중 이라면~
            if (_applicationQuitting == true)
                return;

            if (_instance == null)
            {
                _instance = this as T;

                if (transform.parent != null)
                    transform.parent = null;

                DontDestroyOnLoad(gameObject);
                ChildAwake();
            }
            else if (_instance != this)
            {
                Debug.LogError($"[Singleton] 중복된 {typeof(T).Name} 인스턴스가 이미 존재하여 이 오브젝트를 파괴함~ (오브젝트명: {gameObject.name})");
                Destroy(gameObject);
            }
        }

        //이 싱글톤 클래스의 자식 클래스들이 Awake()에 로직을 추가하고자 할 경우
        //ChildAwake()함수를 오버라이드 한다.
        //물론 자식 클래스의 Awake()함수 안에서 먼저 base.Awake()을 호출해 줘도 되지만
        //개발자가 실수로 base.Awake()을 추가 안 할수도 있으므로 ChildAwake()함수의 오버라이드를 권장한다.
        protected virtual void ChildAwake() { }

        protected virtual void OnApplicationQuit()
        {
            _applicationQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            //인스턴스가 파괴될 때 참조를 해제하여 메모리 누수 방지
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
