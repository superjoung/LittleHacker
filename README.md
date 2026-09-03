# LittleHacker

> 플릭 경로로 숫자와 연산자를 모아 목표값을 완성하는 모바일 퍼즐

플레이어가 격자 안에서 이동하며 숫자와 연산자를 순서대로 수집하고, 제한된 경로로 목표 수식을 만드는 Android용 2D 퍼즐입니다.

- 개발 기간: 2024.04–09
- 팀 구성: 4인
- 현재 저장소 기준: Unity 2021.3.19f1, C#
- 플랫폼: Android

## 정회륜 담당 범위

- JSON 기반 스테이지 데이터 로드와 맵 생성
- 맵 크기에 따른 카메라 영역 계산
- 플릭 입력과 Raycast 기반 이동 경로 판정
- 숫자·연산자 조합과 수식 상태 처리
- 위치·수식·이동 상태를 저장하는 턴 단위 되돌리기

## 핵심 기술 문제와 해결

### 1. 스테이지 제작 데이터와 런타임 오브젝트 결합

스테이지마다 오브젝트 배치 코드를 작성하지 않도록 맵 데이터를 JSON으로 분리했습니다. `MapCreate.Initialize()`가 JSON을 역직렬화하고 런타임 오브젝트를 생성하므로, 스테이지 추가가 코드 수정에 직접 의존하지 않습니다.

관련 코드: [MapCreate.cs](Assets/Scripts/MapCreate/MapCreate.cs), [ObjectData.cs](Assets/Scripts/MapCreate/ObjectData.cs)

### 2. 플릭 경로의 충돌·상호작용 판정

이동 방향마다 벽, 문, 트리거, 아이템을 구분해야 했습니다. LayerMask별 Raycast를 공통 함수로 모아 이동 가능 여부와 상호작용 대상을 같은 방향 입력에서 판정했습니다.

관련 코드: [RayEmission.cs](Assets/Scripts/RayEmission.cs), [Player.cs](Assets/Scripts/Player.cs)

### 3. 턴 단위 되돌리기

단순히 위치만 복원하면 이미 수집한 숫자와 연산 순서가 남습니다. 각 턴에 위치, 이동 종류, 수식 상태를 함께 기록하고 역순으로 복원해 퍼즐 상태를 일관되게 되돌렸습니다.

관련 코드: [Player.cs](Assets/Scripts/Player.cs)

## 선택 이유

- JSON은 맵 제작 데이터와 런타임 로직을 분리해 반복적인 스테이지 제작 비용을 낮춥니다.
- LayerMask Raycast는 격자 이동에서 검사 대상과 규칙을 명시적으로 구분할 수 있습니다.
- Undo는 화면 상태가 아니라 게임 상태를 기록해야 하므로 위치와 수식을 같은 턴 스냅샷으로 다뤘습니다.

## 실행 방법

1. Unity Hub에서 Unity `2021.3.19f1`로 프로젝트를 엽니다.
2. `Assets/Scenes/1.StartScene.unity`를 엽니다.
3. Play Mode로 실행하거나 Android 빌드를 생성합니다.

## 포트폴리오 문서 안내

이 브랜치는 README 설명을 추가한 포트폴리오 버전이며 기존 팀 프로젝트 코드는 보존합니다. 팀원의 작업과 구분되는 정회륜의 구현 범위를 중심으로 작성했습니다.
