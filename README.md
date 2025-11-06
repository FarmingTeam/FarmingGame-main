# FarmingGame V 0.2.0

![Image](https://github.com/user-attachments/assets/066a9d96-20ad-429c-8768-4c39df844215)

# 프로젝트 소개
스파르타코딩클럽 내일배움캠프 - 마지막 프로젝트 Unity 기반의 생활시뮬레이션 기반의 농장 경영 게임으로 자신만의 농장을 가꾸며 소소한 성장의 재미를 제공합니다

# 개발 기간
2025.09.09 (화) ~ 2025.10.31(금)

# 기획팀 소개

<table align="center">
  <tr>
    <td align="center" width="230px">
      <br/>
      <b>기획팀장 [박용규]</b>
      <br/>
      <sub>기획/기획서 작성 & 수정 , 후반 일정관리 , 총무 , QA , 유저 테스트 문서화</sub>
      <br/>
      <a href="https://github.com/dragonk1631/">
        <img src="https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white"/>
      </a>
    </td>
    <td align="center" width="220px">
      <br/>
      <b>팀원 [황의영]</b>
      <br/>
      <sub>기획/콘텐츠 기획, 밸런스 기획 , QA</sub>
      <br/>
      <a href="https://github.com/i1i1i1i1ii1i/">
        <img src="https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white"/>
      </a>
    </td>
  </tr>
</table>


# 개발팀 소개

<table align="center">
  <tr>
    <td align="center" width="230px">
      <br/>
      <b>개발팀장 [문장원]</b>
      <br/>
      <sub>싱글톤 관리 및 저장 중앙총괄, 맵 및 인터렉션, 미니게임(QTE, 요리, 낚시), 시간 시스템</sub>
      <br/>
      <a href="https://github.com/NineTheDivine">
        <img src="https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white"/>
      </a>
    </td>
    <td align="center" width="220px">
      <br/>
      <b>팀원 [김우민]</b>
      <br/>
      <sub>개발/NPC , 퀘스트 , 상점 , 다이얼로그</sub>
      <br/>
      <a href="https://github.com/woomin0011">
        <img src="https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white"/>
      </a>
    </td>
    <td align="center" width="220px">
      <br/>
      <b>팀원 [김나경]</b>
      <br/>
      <sub>인벤토리 시스템 전반 및 아이템 데이터 설계, 도감 시스템, 도구 업그레이드 시스템, 퀵슬롯 시스템</sub>
      <br/>
      <a href="https://github.com/nakyung71">
        <img src="https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white"/>
      </a>
    </td>
    <td align="center" width="220px">
      <br/>
      <b>팀원 [구슬기]</b>
      <br/>
      <sub>개발/맡은 일 채워넣기</sub>
      <br/>
      <a href="https://github.com/sulg16">
        <img src="https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white"/>
      </a>
    </td>
    <td align="center" width="220px">
      <br/>
      <b>팀원 [차광호]</b>
      <br/>
      <sub>개발/플레이어 동작 및 애니메이션, 튜토리얼, 인풋시스템, 트랜지션</sub>
      <br/>
      <a href="https://github.com/gwang910">
        <img src="https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white"/>
      </a>
    </td>
  </tr>
</table>

# Development Enviroment
Language: C#

Engine: Unity 2022.3.62f2

IDE : Visual Studio 2022


## 게임 흐름

타이틀 화면 : 뉴게임 , 게임 로드

집내부 : 수면 , 요리

농장 : 개간 , 작물재배 , 작물수확 , 작물판매

강가 : 낚시

마을 : 퀘스트 , 상점 이용
## 조작법
| 동작             | 키보드 입력     |
|------------------|-----------------|
| 이동             | W / A / S / D   |
| 달리기             | Shift           |
| 개간 및 작물재배    | Left mouse click   |
| QTE             |  Left mouse click |
| 낚시            | Left mouse click |
| 상호작용         | E / Left mouse click  |
| NPC대화         | E / Left mouse click   |
| 메뉴 열기        | TAB            |

# 게임 플레이



![Image](https://github.com/user-attachments/assets/5cc3ca10-ee78-43f6-bb1e-9cac9f4e2aca)

![Image](https://github.com/user-attachments/assets/ac2b1d27-5dfd-44c2-b324-e5ed4307d7ec)

![Image](https://github.com/user-attachments/assets/3562618c-c197-4bca-866b-fcdde8438e23)

![Image](https://github.com/user-attachments/assets/d2bd24ab-b2b1-4776-a048-2b7a397ffc5c)

![Image](https://github.com/user-attachments/assets/f79c3c84-26c2-4d74-89a6-9070279c0868)

![Image](https://github.com/user-attachments/assets/4dd286a2-8f80-48cf-9c56-ff50aa41b77c)

![Image](https://github.com/user-attachments/assets/acbe1c6c-5c08-4fbe-a5ab-fda1362bac44)

# 주요 기능

## 플레이어 시스템
### 1. 조작방법
  - W / 위쪽 화살표 : 위로 이동
  - S / 아래쪽 화살표 : 아래로 이동
  - A / 왼쪽 화살표 : 왼쪽으로 이동
  - D / 오른쪽 화살표 : 오른쪽으로 이동
  - 탭 : 메뉴창 열기 / 닫기
  - Shift : 달리기
  - E : 상호작용
  - 숫자 : 도구 퀵슬롯
### 2. 도구에 따른 상호작용 대상
  - 빈손 : 꽃, 버섯, 풀
  - 씨앗주머니(1) : 갈아진 땅
  - 도끼(2) : 나무
  - 곡괭이(3) : 돌
  - 괭이(4) : 갈아지지 않은 땅
  - 물뿌리개(5) : 물 타일, 우물에서는 물뜨기, 갈아진 땅에서는 물뿌리기
  - 낫(6) : 풀, 다 자란 작물
  - 낚싯대(7) : 물 타일

## 맵 시스템
  - 세가지 레이어로 각 타일을 구분
    
  - 청크 단위의 Scriptable Object를 그래프 자료구조로 관리

  - TimeManager와 결합하여 씨앗 성장 구현
## 인벤토리 시스템
  - 슬롯 리스트를 기반으로 아이템을 추가·삭제하고 같은 아이템이면 수량을 합산하며 빈 슬롯을 찾아 배치

  - 인벤토리 정렬, 저장·로드(JSON 직렬화), 아이템 검색 등 아이템 상태를 일관되게 관리

  - UI와 연동하여 슬롯 상태가 바뀌면 즉시 갱신되도록 이벤트 기반으로 동작

## 요리 시스템
  - RectTransform의 Anchor 및 Pivot을 활용한 크기변화
    
  - 코루틴을 활용한 플레이어 입력에 따른 변화처리
## 낚시 시스템
  - 랜덤 위치를 정해 목표 랜덤 이동 구현

## NPC 시스템
  - 모든 NPC 대사와 선택지가 CSV 기반 데이터로 관리되어 기획/변경이 편리
    
  - NPC별 대화 분류 및 상태별(일반, 퀘스트) 대화 지원
    
  - 상호작용 거리, 퀘스트 상태(이모지의 색이 변경)별 아이콘 표시
## 퀘스트 시스템
  - 선행/후속 퀘스트 자동 해금과 진행 상황 추적 기능

  - CSV에서 다양한 퀘스트 조건(수집, 특정 아이템 등) 정의 가능
    
  - 보상 지급(골드, 아이템, 호감도 등) 및 범위 기반 아이템 차감 지원
    
  - 퀘스트 저장/불러오기, 상태 동기화
## UI/UX

## 저장 시스템
  - 게임 내 싱글톤으로 캐싱된 데이터를 JsonUtility를 활용하여 저장 및 로드
    
  - 파일 정보 확인 및 스크린샷 저장 기능

  - 버전에 따른 충돌방지를 위한 세이브데이터 삭제기능

## 사운드 구성

## 주요구성
### 1. 튜토리얼
  - 조작키
  - 농사
  - 스태미나
  - QTE 미니게임
  - 인벤토리
  - 도감
  - 시스템
  - 낚시
  - 상호작용
  - 상점
  - 퀵슬롯
  - 물뿌리개
  - 지도
  - 판매상자
  - 달리기
  - 요리
  - 축제
  - 강제취침시간
  - 적절한 상황에서 튜토리얼 팝업이 생성되며, 집앞 게시판에서 다시 확인이 가능합니다.
