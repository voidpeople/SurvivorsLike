# SurvivorsLike

> **탕탕특공대(Survivor.io) 스타일의 3D 모바일 뱀서라이크(Vampire Survivors-like) 게임**
> Unity 6.3 LTS 기반 · 개인 포트폴리오 프로젝트

<!-- TODO: 아래에 대표 플레이 GIF 또는 스크린샷을 넣으세요. 심사관이 가장 먼저 보는 영역입니다. -->
<!-- 예) ![게임플레이](Docs/Images/gameplay.gif) -->

![Unity](https://img.shields.io/badge/Unity-6.3%20LTS-000000?logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Android%20%2F%20iOS-3DDC84)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow)

---

## 📖 프로젝트 소개

수십~수백 마리의 적이 몰려오는 필드에서 자동 공격으로 생존하며 성장하는 **3D 모바일 뱀서라이크** 게임입니다.
원작 *탕탕특공대*의 핵심 게임성(자동 전투, 스킬 조합, 웨이브 생존, 챕터 진행)을 **3D 그래픽**과 **모바일 실무 아키텍처**로 재구현하는 데 목표를 두었습니다.

이 프로젝트는 **단순한 기능 구현이 아니라, 중소 규모 모바일 게임 개발 현장의 설계 표준**을 적용하는 데 중점을 두고 있습니다.

| 항목 | 내용 |
|---|---|
| 장르 | 3D 뱀서라이크 / 생존 액션 |
| 플랫폼 | 모바일 (Android / iOS) |
| 엔진 | Unity 6.3 LTS |
| 개발 형태 | 개인 포트폴리오 (1인 개발) |
| 핵심 목표 | 실무 수준 아키텍처 · 확장 가능한 시스템 설계 |

---

## 🛠 기술 스택

| 분류 | 사용 기술 |
|---|---|
| **엔진 / 언어** | Unity 6.3 LTS, C# |
| **비동기** | UniTask 2.5.10 |
| **반응형 프로그래밍** | R3 1.3.0 |
| **에셋 관리** | Addressables 2.9.1 |
| **연출 / 트윈** | DOTween Pro |
| **에디터 확장** | Tri Inspector |
| **UI 텍스트** | TextMeshPro (NanumGothic) |
| **백엔드** | Firebase (Authentication · Firestore · Analytics) |

---

## 🏛 아키텍처 & 설계 하이라이트

> 이 프로젝트가 가장 공들인 부분입니다. **확장성·유지보수성·테스트 용이성**을 기준으로 설계 패턴을 적용했습니다.

### 1. MVP 패턴 기반 UI
로비/인게임 UI를 **Model · View · Presenter**로 분리하여 UI 로직과 표현을 격리했습니다.
데이터 바인딩에는 **R3(Reactive Extensions)** 를 활용해 상태 변화를 선언적으로 처리합니다.

```
UI/Lobby/
├── Models/        ← 상태·데이터 (BattlePanelModel, ChapterSelectPanelModel, LobbyTabModel)
├── Views/         ← 표현 전담, 로직 없음 (BattlePanelView ...)
└── Presenters/    ← Model↔View 중재, 입력 처리 (BattlePanelPresenter ...)
```

### 2. FSM(유한 상태 머신) 기반 적 AI
일반 몬스터와 보스를 **상태 패턴(State Pattern)** 으로 구현하여 AI 행동을 모듈화했습니다.

```
Enemy AI
├── EnemyFSM          ← 상태 전이 관리
│   ├── IdleState
│   ├── ChaseState
│   ├── AttackState
│   └── DeadState
└── BossFSM           ← 보스 전용 확장 상태 머신
```

### 3. Factory 패턴 기반 스킬 시스템 (13종)
**SkillFactory** 로 스킬 생성을 중앙화하고, 공통 `SkillBase`를 상속한 **13종의 다양한 스킬**을 데이터(ScriptableObject) 구동 방식으로 구현했습니다.

```
SkillFactory → SkillBase 상속
├── LinearProjectileSkill   (직선 투사체)
├── BouncingProjectileSkill (튕기는 투사체)
├── StickyProjectileSkill   (부착 투사체)
├── BoomerangSkill          (부메랑)
├── OrbitalSkill            (회전 궤도)
├── LaserSkill              (레이저)
├── AreaSkill / AuraSkill   (장판 / 오라)
├── MeleeSkill              (근접)
├── SummonSkill             (소환)
└── TrapSkill               (설치형)
```

### 4. 인터페이스 기반 컴포지션 설계
상속 대신 **인터페이스 조합(Composition)** 으로 캐릭터 기능을 분리해 결합도를 낮췄습니다.

```
IAlive · IMovementListener · ISkillOwner ·
ITargetable · ITargetProvider · ITargetListener · ITickable
```

### 5. 오브젝트 풀링 (성능 최적화)
다수의 투사체·적이 생성/소멸되는 뱀서라이크 특성상, **PoolManager + IPoolable** 로 GC 부하를 최소화했습니다.
투사체는 **ProjectileManager** 가 풀을 전담 관리합니다.

### 6. 이벤트 버스 (느슨한 결합)
**InGameEventBus** 로 인게임 시스템 간 통신을 디커플링하여, 모듈 간 직접 참조를 제거했습니다.

### 7. 매니저 계층 & 데이터 구동
- **GameManager / InGameStateManager** — 게임 흐름·상태 관리
- **DataManager** — ScriptableObject 기반 데이터 테이블 (구글 시트 연동 대비 camelCase 필드)
- **FirebaseManager** — 익명 인증 + Firestore 유저 데이터 동기화
- **SoundManager / UIManager / PopupManager / SystemUIManager** — 시스템별 책임 분리
- **SceneLoader / FadeManager** — 씬 전환 및 연출

---

## 🎮 주요 구현 시스템

- ✅ **자동 전투 시스템** — 타겟 탐색(ITargetProvider) 기반 자동 조준·발사
- ✅ **웨이브 / 스폰 시스템** — WaveManager · EnemySpawner 기반 시간별 적 스폰
- ✅ **스킬 시스템** — 13종 스킬, 데이터 구동 + Factory 생성
- ✅ **보스 전투** — 전용 FSM 기반 패턴
- ✅ **챕터 선택 로비** — MVP + 캐러셀 스크롤 UI
- ✅ **경험치 / 레벨업** — 인게임 성장 루프
- ✅ **저장 / 클라우드 동기화** — SaveManager + Firebase Firestore
- ✅ **패치 / 어드레서블 로드** — PatchManager + Addressables

---

## 📂 프로젝트 구조

```
Assets/_Project/                ← 게임 콘텐츠 루트 (엔티티 단위 그룹화)
├── Scripts/
│   ├── Core/                   ← SingletonMonoBehaviour, EventBus, Save, GameSessionData
│   ├── Managers/               ← 전역 매니저 계층 (Game, Data, Firebase, UI ...)
│   ├── Character/              ← 플레이어·적·보스 (FSM, Health, 인터페이스)
│   ├── Skill/                  ← 스킬 시스템 (Factory + 13종)
│   ├── Projectile/             ← 투사체 로직
│   ├── UI/                     ← MVP UI (Lobby / InGame / SystemUI)
│   ├── Data/                   ← ScriptableObject 데이터 테이블
│   ├── Map / Camera / Audio / Input / VFX / Utils ...
│   └── Editor/                 ← 에디터 확장 툴
├── Characters/                 ← 캐릭터 리소스 (Models / Materials / Prefabs / Animations)
├── Data/                       ← SO 에셋 (Enemy / Skill / Wave / Chapter ...)
├── UI/                         ← UI 프리팹 (Lobby / InGame / System ...)
├── VFX / Map / Audio / Fonts / Scenes
└── ...
```

---

## 🚧 개발 현황 & 로드맵

이 프로젝트는 **현재 활발히 개발이 진행 중**입니다.

**구현 완료**
- [x] 코어 아키텍처 (매니저 계층 · 이벤트 버스 · 풀링)
- [x] 적 AI(FSM) · 웨이브 시스템
- [x] 스킬 시스템 (13종)
- [x] MVP 기반 로비 / 인게임 UI
- [x] Firebase 연동 (익명 인증 · Firestore 저장)

**진행 / 예정**
- [ ] 스킬 강화·조합(시너지) 시스템 고도화
- [ ] 메타 성장(장비·캐릭터 육성) 콘텐츠
- [ ] 밸런스 데이터 구글 시트 파이프라인
- [ ] 사운드·VFX 폴리싱
- [ ] 빌드 최적화 (Addressables 그룹 분리)

---

## 👤 개발자

<!-- TODO: 아래 정보를 채워주세요 -->
- **이름**: (작성)
- **역할**: 클라이언트 프로그래머 (기획·설계·구현 전반)
- **GitHub**: [@voidpeople](https://github.com/voidpeople)
- **연락처**: (작성)

---

## 📌 참고 사항

- 본 저장소는 포트폴리오 목적의 **모작(模作) 프로젝트**이며, 상업적 이용 목적이 아닙니다.
- 일부 유료 에셋(DOTween Pro 등)과 백엔드 설정 파일(`google-services.json`)은 라이선스/보안상 저장소에서 제외되어 있습니다.
- 원작 *탕탕특공대 / Survivor.io* 의 IP는 각 권리자에게 있습니다.
