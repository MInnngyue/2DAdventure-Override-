# C#核心概念 - Unity开发必备（上篇）

## 目录

1. [委托（Delegate）](#1-委托delegate)
2. [事件（Event）](#2-事件event)
3. [Lambda表达式](#3-lambda表达式)
4. [协程（Coroutine）](#4-协程coroutine)

---

## 1. 委托（Delegate）

### 什么是委托？

委托就像是"方法的容器"或"方法的引用"。你可以把它想象成一个变量，但这个变量存储的不是数字或字符串，而是一个方法。

**生活中的类比：**
想象你是一个老板，你有很多员工。你不需要亲自做每件事，而是把任务"委托"给员工。委托就是这样一个机制：你告诉程序"当某件事发生时，去调用这个方法"。

### 为什么需要委托？

在没有委托之前，如果你想让一个方法在特定时候被调用，你需要直接写死调用代码。但有了委托，你可以：
- 动态决定调用哪个方法
- 让一个事件触发多个方法
- 将方法作为参数传递给其他方法

### 基本语法 - 逐步拆解

让我们一步一步理解委托：

#### 第一步：声明委托类型

```csharp
public delegate void MyDelegate(string message);
```

**详细解释：**

这行代码在做什么？它在定义一个"规则"或"模板"。

- `public`：这个委托可以被其他类访问
- `delegate`：关键字，告诉C#这是一个委托
- `void`：这个委托指向的方法不返回任何值
- `MyDelegate`：委托的名字（你可以随便起名）
- `(string message)`：这个委托指向的方法必须接受一个string类型的参数

**类比理解：**
这就像你在招聘时写的职位要求：
```
职位：消息处理员（MyDelegate）
要求：必须能接收一条文本消息（string message）
      处理完不需要返回结果（void）
```

#### 第二步：创建符合委托签名的方法

```csharp
public void PrintMessage(string message)
{
    Debug.Log(message);
}
```

**详细解释：**

这是一个普通的方法，但它"符合"我们刚才定义的委托规则：
- 返回类型是`void` ✓
- 接受一个`string`参数 ✓

**类比理解：**
这就像一个符合职位要求的员工：
- 名字叫 PrintMessage
- 能接收文本消息
- 会把消息打印出来

#### 第三步：使用委托

```csharp
MyDelegate del = PrintMessage;
```

**详细解释：**

这行代码在做什么？

1. `MyDelegate del`：创建一个委托类型的变量，名字叫`del`
2. `= PrintMessage`：把`PrintMessage`方法赋值给这个委托变量

**注意：** 这里写的是`PrintMessage`，不是`PrintMessage()`。我们不是在调用方法，而是在引用方法本身。

**类比理解：**
```
老板（del）= 雇佣了员工（PrintMessage）
```

#### 第四步：通过委托调用方法

```csharp
del("Hello World");
```

**详细解释：**

现在我们通过委托来调用方法：

1. `del`：这是我们的委托变量
2. `("Hello World")`：传递参数给委托

**执行流程：**
```
del("Hello World")
    ↓
调用 PrintMessage("Hello World")
    ↓
执行 Debug.Log("Hello World")
    ↓
输出：Hello World
```

**类比理解：**
```
老板说："去处理这条消息：Hello World"
    ↓
员工PrintMessage收到任务
    ↓
员工执行：打印"Hello World"
```

### 完整示例 - 带详细注释

```csharp
using UnityEngine;

public class DelegateExample : MonoBehaviour
{
    // 第一步：声明委托类型
    // 这个委托可以指向任何"接受string参数、不返回值"的方法
    public delegate void MessageHandler(string msg);
    
    void Start()
    {
        // 第二步：创建委托实例，指向PrintMessage方法
        MessageHandler handler = PrintMessage;
        
        // 第三步：通过委托调用方法
        handler("你好，世界！");  // 输出：你好，世界！
        
        // 也可以改变委托指向的方法
        handler = PrintUpperCase;
        handler("hello");  // 输出：HELLO
    }
    
    // 符合委托签名的方法1
    void PrintMessage(string msg)
    {
        Debug.Log(msg);
    }
    
    // 符合委托签名的方法2
    void PrintUpperCase(string msg)
    {
        Debug.Log(msg.ToUpper());
    }
}
```

**执行流程分析：**

```
1. handler = PrintMessage
   [handler] → [PrintMessage方法]

2. handler("你好，世界！")
   → 调用PrintMessage("你好，世界！")
   → 输出：你好，世界！

3. handler = PrintUpperCase
   [handler] → [PrintUpperCase方法]

4. handler("hello")
   → 调用PrintUpperCase("hello")
   → 输出：HELLO
```

### 多播委托 - 一次调用多个方法

委托的强大之处在于可以同时指向多个方法：

```csharp
public class MulticastExample : MonoBehaviour
{
    public delegate void GameEventHandler();
    
    void Start()
    {
        GameEventHandler onGameStart = null;
        
        // 添加第一个方法
        onGameStart += LoadLevel;
        
        // 添加第二个方法
        onGameStart += PlayMusic;
        
        // 添加第三个方法
        onGameStart += ShowUI;
        
        // 一次调用，执行所有方法
        onGameStart();
        
        // 输出：
        // 加载关卡
        // 播放音乐
        // 显示UI
    }
    
    void LoadLevel()
    {
        Debug.Log("加载关卡");
    }
    
    void PlayMusic()
    {
        Debug.Log("播放音乐");
    }
    
    void ShowUI()
    {
        Debug.Log("显示UI");
    }
}
```

**执行流程：**

```
onGameStart += LoadLevel
[onGameStart] → [LoadLevel]

onGameStart += PlayMusic
[onGameStart] → [LoadLevel] → [PlayMusic]

onGameStart += ShowUI
[onGameStart] → [LoadLevel] → [PlayMusic] → [ShowUI]

onGameStart()
→ 依次调用：LoadLevel() → PlayMusic() → ShowUI()
```

### Unity实际应用：敌人死亡通知系统

```csharp
public class Enemy : MonoBehaviour
{
    // 声明委托类型：当敌人死亡时，通知其他系统
    public delegate void DeathHandler(Enemy enemy);
    
    // 创建委托实例
    public DeathHandler onDeath;
    
    public int scoreValue = 100;
    
    public void Die()
    {
        Debug.Log($"{gameObject.name} 死亡了");
        
        // 如果有订阅者，通知它们
        if(onDeath != null)
        {
            onDeath(this);  // 把自己（Enemy对象）传递给订阅者
        }
        
        Destroy(gameObject);
    }
}

public class GameManager : MonoBehaviour
{
    private int totalScore = 0;
    
    void Start()
    {
        // 找到场景中的所有敌人
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        
        foreach(Enemy enemy in enemies)
        {
            // 订阅每个敌人的死亡事件
            enemy.onDeath += OnEnemyDeath;
            enemy.onDeath += AddScore;
        }
    }
    
    void OnEnemyDeath(Enemy enemy)
    {
        Debug.Log($"游戏管理器收到通知：{enemy.gameObject.name} 已死亡");
    }
    
    void AddScore(Enemy enemy)
    {
        totalScore += enemy.scoreValue;
        Debug.Log($"增加分数：{enemy.scoreValue}，总分：{totalScore}");
    }
}
```

**执行流程详解：**

```
1. 游戏开始
   Enemy1.onDeath → [OnEnemyDeath] → [AddScore]
   Enemy2.onDeath → [OnEnemyDeath] → [AddScore]

2. Enemy1.Die() 被调用
   ↓
   onDeath(this) 执行
   ↓
   OnEnemyDeath(Enemy1) 被调用 → 输出："游戏管理器收到通知：Enemy1 已死亡"
   ↓
   AddScore(Enemy1) 被调用 → 输出："增加分数：100，总分：100"
   ↓
   Destroy(Enemy1)

3. Enemy2.Die() 被调用
   ↓
   （重复上述流程）
```


### 练习题1：创建血量系统

**任务：** 创建一个血量系统，当血量改变时通知UI更新

```csharp
public class HealthSystem : MonoBehaviour
{
    // TODO: 声明一个委托类型 HealthChangeHandler
    // 要求：接受两个float参数（当前血量、最大血量），无返回值
    
    // TODO: 创建委托实例 onHealthChange
    
    private float health = 100f;
    private float maxHealth = 100f;
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health < 0) health = 0;
        
        // TODO: 如果onHealthChange不为null，调用它
        // 传递参数：health, maxHealth
    }
}
```

**答案：**

```csharp
public class HealthSystem : MonoBehaviour
{
    // 声明委托类型
    public delegate void HealthChangeHandler(float currentHealth, float maxHealth);
    
    // 创建委托实例
    public HealthChangeHandler onHealthChange;
    
    private float health = 100f;
    private float maxHealth = 100f;
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health < 0) health = 0;
        
        // 调用委托，通知所有订阅者
        if(onHealthChange != null)
        {
            onHealthChange(health, maxHealth);
        }
    }
}

// UI管理器订阅血量变化
public class UIManager : MonoBehaviour
{
    void Start()
    {
        HealthSystem healthSystem = FindObjectOfType<HealthSystem>();
        
        // 订阅血量变化事件
        healthSystem.onHealthChange += UpdateHealthBar;
        healthSystem.onHealthChange += ShowDamageEffect;
    }
    
    void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float percentage = currentHealth / maxHealth;
        Debug.Log($"更新血条：{percentage * 100}%");
    }
    
    void ShowDamageEffect(float currentHealth, float maxHealth)
    {
        Debug.Log("显示受伤特效");
    }
}
```

**执行流程：**

```
1. 玩家受到10点伤害
   ↓
   TakeDamage(10) 被调用
   ↓
   health = 100 - 10 = 90
   ↓
   onHealthChange(90, 100) 被调用
   ↓
   UpdateHealthBar(90, 100) → 输出："更新血条：90%"
   ↓
   ShowDamageEffect(90, 100) → 输出："显示受伤特效"
```

### 练习题2：技能冷却通知

**任务：** 创建一个技能系统，当技能冷却完成时通知玩家

```csharp
public class Skill : MonoBehaviour
{
    // TODO: 声明委托类型 CooldownCompleteHandler
    // 要求：接受一个string参数（技能名称），无返回值
    
    // TODO: 创建委托实例 onCooldownComplete
    
    public string skillName = "火球术";
    public float cooldownTime = 5f;
    
    public void UseSkill()
    {
        Debug.Log($"使用技能：{skillName}");
        StartCoroutine(CooldownCoroutine());
    }
    
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        
        // TODO: 冷却完成，调用委托通知订阅者
        // 传递参数：skillName
    }
}
```

**答案：**

```csharp
public class Skill : MonoBehaviour
{
    // 声明委托类型
    public delegate void CooldownCompleteHandler(string skillName);
    
    // 创建委托实例
    public CooldownCompleteHandler onCooldownComplete;
    
    public string skillName = "火球术";
    public float cooldownTime = 5f;
    
    public void UseSkill()
    {
        Debug.Log($"使用技能：{skillName}");
        StartCoroutine(CooldownCoroutine());
    }
    
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        
        // 冷却完成，通知订阅者
        if(onCooldownComplete != null)
        {
            onCooldownComplete(skillName);
        }
    }
}

// 使用示例
public class Player : MonoBehaviour
{
    void Start()
    {
        Skill skill = GetComponent<Skill>();
        
        // 订阅冷却完成事件
        skill.onCooldownComplete += OnSkillReady;
        skill.onCooldownComplete += PlayReadySound;
    }
    
    void OnSkillReady(string skillName)
    {
        Debug.Log($"{skillName} 冷却完成！");
    }
    
    void PlayReadySound(string skillName)
    {
        Debug.Log($"播放 {skillName} 就绪音效");
    }
}
```

---

## 2. 事件（Event）

### 什么是事件？

事件是基于委托的，但提供了更好的"保护"。如果说委托是一个公开的员工名单，那么事件就是一个有门禁的员工名单。

### 委托 vs 事件的区别

让我们通过代码看区别：

**使用委托：**

```csharp
public class Player : MonoBehaviour
{
    public delegate void JumpHandler();
    public JumpHandler OnJump;  // 使用委托
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnJump();  // 调用委托
        }
    }
}

public class BadGuy : MonoBehaviour
{
    void Start()
    {
        Player player = FindObjectOfType<Player>();
        
        // 危险！外部可以做这些事：
        player.OnJump = null;  // 清空所有订阅者
        player.OnJump = MyMethod;  // 替换所有订阅者
        player.OnJump();  // 直接调用
    }
    
    void MyMethod() { }
}
```

**使用事件：**

```csharp
public class Player : MonoBehaviour
{
    public delegate void JumpHandler();
    public event JumpHandler OnJump;  // 使用事件（加了event关键字）
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnJump?.Invoke();  // 只能在类内部调用
        }
    }
}

public class GoodGuy : MonoBehaviour
{
    void Start()
    {
        Player player = FindObjectOfType<Player>();
        
        // 安全！外部只能做这些事：
        player.OnJump += MyMethod;  // 订阅 ✓
        player.OnJump -= MyMethod;  // 取消订阅 ✓
        
        // 以下操作会报错：
        // player.OnJump = null;  // ✗ 编译错误
        // player.OnJump();  // ✗ 编译错误
    }
    
    void MyMethod() { }
}
```

**总结：**
- **委托**：外部可以 `=`（赋值）、`+=`（订阅）、`-=`（取消）、直接调用
- **事件**：外部只能 `+=`（订阅）、`-=`（取消），不能赋值或调用

### 事件的基本语法

```csharp
public class Player : MonoBehaviour
{
    // 第一步：声明委托类型
    public delegate void JumpHandler();
    
    // 第二步：基于委托创建事件（加event关键字）
    public event JumpHandler OnJump;
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    
    void Jump()
    {
        Debug.Log("玩家跳跃");
        
        // 第三步：触发事件（只能在类内部）
        if(OnJump != null)
        {
            OnJump();
        }
        
        // 或者使用简化写法
        OnJump?.Invoke();
    }
}
```

**详细解释：**

1. **声明委托类型**：定义事件的"规则"
2. **创建事件**：用`event`关键字修饰委托
3. **触发事件**：只能在声明事件的类内部调用

**`?.Invoke()` 的含义：**
- `?`：如果OnJump不为null
- `.Invoke()`：调用事件
- 等价于：`if(OnJump != null) OnJump();`

### 使用Action简化事件声明

C#提供了内置的委托类型`Action`，可以简化代码：

```csharp
public class Player : MonoBehaviour
{
    // 不需要声明委托类型，直接使用Action
    public event Action OnJump;              // 无参数
    public event Action<int> OnScoreChange;  // 一个int参数
    public event Action<float, float> OnHealthChange;  // 两个float参数
    
    void Jump()
    {
        OnJump?.Invoke();  // 触发事件
    }
    
    void AddScore(int points)
    {
        OnScoreChange?.Invoke(points);
    }
    
    void TakeDamage(float damage, float currentHealth)
    {
        OnHealthChange?.Invoke(damage, currentHealth);
    }
}
```

**Action的类型：**
- `Action`：无参数，无返回值
- `Action<T>`：一个参数，无返回值
- `Action<T1, T2>`：两个参数，无返回值
- `Action<T1, T2, T3>`：三个参数，无返回值
- ...最多16个参数


### Unity实际应用：完整的游戏事件系统

```csharp
public class GameEvents : MonoBehaviour
{
    // 单例模式，方便全局访问
    public static GameEvents instance;
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 定义各种游戏事件
    public event Action OnGameStart;           // 游戏开始
    public event Action OnGameOver;            // 游戏结束
    public event Action<int> OnEnemyKilled;    // 敌人被杀（传递分数）
    public event Action<int> OnScoreChanged;   // 分数改变
    public event Action<string> OnLevelComplete;  // 关卡完成（传递关卡名）
    
    // 触发事件的方法
    public void TriggerGameStart()
    {
        Debug.Log("触发游戏开始事件");
        OnGameStart?.Invoke();
    }
    
    public void TriggerGameOver()
    {
        Debug.Log("触发游戏结束事件");
        OnGameOver?.Invoke();
    }
    
    public void TriggerEnemyKilled(int score)
    {
        Debug.Log($"触发敌人被杀事件，分数：{score}");
        OnEnemyKilled?.Invoke(score);
    }
}

// UI管理器订阅事件
public class UIManager : MonoBehaviour
{
    void OnEnable()
    {
        // 订阅事件
        GameEvents.instance.OnGameStart += ShowGameUI;
        GameEvents.instance.OnGameOver += ShowGameOverUI;
        GameEvents.instance.OnEnemyKilled += UpdateKillCount;
    }
    
    void OnDisable()
    {
        // 取消订阅（重要！防止内存泄漏）
        GameEvents.instance.OnGameStart -= ShowGameUI;
        GameEvents.instance.OnGameOver -= ShowGameOverUI;
        GameEvents.instance.OnEnemyKilled -= UpdateKillCount;
    }
    
    void ShowGameUI()
    {
        Debug.Log("显示游戏UI");
    }
    
    void ShowGameOverUI()
    {
        Debug.Log("显示游戏结束UI");
    }
    
    void UpdateKillCount(int score)
    {
        Debug.Log($"更新击杀数，获得分数：{score}");
    }
}

// 音频管理器也订阅事件
public class AudioManager : MonoBehaviour
{
    void OnEnable()
    {
        GameEvents.instance.OnGameStart += PlayBackgroundMusic;
        GameEvents.instance.OnGameOver += PlayGameOverSound;
        GameEvents.instance.OnEnemyKilled += PlayKillSound;
    }
    
    void OnDisable()
    {
        GameEvents.instance.OnGameStart -= PlayBackgroundMusic;
        GameEvents.instance.OnGameOver -= PlayGameOverSound;
        GameEvents.instance.OnEnemyKilled -= PlayKillSound;
    }
    
    void PlayBackgroundMusic()
    {
        Debug.Log("播放背景音乐");
    }
    
    void PlayGameOverSound()
    {
        Debug.Log("播放游戏结束音效");
    }
    
    void PlayKillSound(int score)
    {
        Debug.Log("播放击杀音效");
    }
}
```

**执行流程：**

```
1. 游戏开始
   GameEvents.instance.TriggerGameStart()
   ↓
   OnGameStart?.Invoke()
   ↓
   UIManager.ShowGameUI() → 输出："显示游戏UI"
   ↓
   AudioManager.PlayBackgroundMusic() → 输出："播放背景音乐"

2. 敌人被杀
   GameEvents.instance.TriggerEnemyKilled(100)
   ↓
   OnEnemyKilled?.Invoke(100)
   ↓
   UIManager.UpdateKillCount(100) → 输出："更新击杀数，获得分数：100"
   ↓
   AudioManager.PlayKillSound(100) → 输出："播放击杀音效"
```

### 为什么要在OnDisable中取消订阅？

这是一个重要的概念：**防止内存泄漏**

```csharp
public class BadExample : MonoBehaviour
{
    void OnEnable()
    {
        GameEvents.instance.OnGameStart += ShowUI;
    }
    
    // 没有取消订阅！
    // 当这个对象被销毁时，事件仍然持有它的引用
    // 导致对象无法被垃圾回收 → 内存泄漏
    
    void ShowUI()
    {
        Debug.Log("显示UI");
    }
}

public class GoodExample : MonoBehaviour
{
    void OnEnable()
    {
        GameEvents.instance.OnGameStart += ShowUI;
    }
    
    void OnDisable()
    {
        // 正确！取消订阅
        GameEvents.instance.OnGameStart -= ShowUI;
    }
    
    void ShowUI()
    {
        Debug.Log("显示UI");
    }
}
```

### 练习题1：商店购买系统

**任务：** 创建一个商店系统，当购买物品时触发事件通知其他系统

```csharp
public class Shop : MonoBehaviour
{
    // TODO: 使用Action创建事件 OnItemPurchased
    // 参数：string itemName（物品名称）, int price（价格）
    
    public void BuyItem(string itemName, int price)
    {
        Debug.Log($"购买了 {itemName}，花费 {price} 金币");
        
        // TODO: 触发事件，传递 itemName 和 price
    }
}

// TODO: 创建 InventoryManager 类
// 订阅 OnItemPurchased 事件
// 实现 AddToInventory 方法，将物品添加到背包

// TODO: 创建 AudioManager 类
// 订阅 OnItemPurchased 事件
// 实现 PlayPurchaseSound 方法，播放购买音效
```

**答案：**

```csharp
public class Shop : MonoBehaviour
{
    // 创建事件
    public event Action<string, int> OnItemPurchased;
    
    public void BuyItem(string itemName, int price)
    {
        Debug.Log($"购买了 {itemName}，花费 {price} 金币");
        
        // 触发事件
        OnItemPurchased?.Invoke(itemName, price);
    }
}

public class InventoryManager : MonoBehaviour
{
    void OnEnable()
    {
        Shop shop = FindObjectOfType<Shop>();
        shop.OnItemPurchased += AddToInventory;
    }
    
    void OnDisable()
    {
        Shop shop = FindObjectOfType<Shop>();
        if(shop != null)
        {
            shop.OnItemPurchased -= AddToInventory;
        }
    }
    
    void AddToInventory(string itemName, int price)
    {
        Debug.Log($"将 {itemName} 添加到背包");
    }
}

public class AudioManager : MonoBehaviour
{
    void OnEnable()
    {
        Shop shop = FindObjectOfType<Shop>();
        shop.OnItemPurchased += PlayPurchaseSound;
    }
    
    void OnDisable()
    {
        Shop shop = FindObjectOfType<Shop>();
        if(shop != null)
        {
            shop.OnItemPurchased -= PlayPurchaseSound;
        }
    }
    
    void PlayPurchaseSound(string itemName, int price)
    {
        Debug.Log($"播放购买音效：叮！");
    }
}
```

**执行流程：**

```
Shop.BuyItem("生命药水", 50)
↓
输出："购买了 生命药水，花费 50 金币"
↓
OnItemPurchased?.Invoke("生命药水", 50)
↓
InventoryManager.AddToInventory("生命药水", 50)
→ 输出："将 生命药水 添加到背包"
↓
AudioManager.PlayPurchaseSound("生命药水", 50)
→ 输出："播放购买音效：叮！"
```

### 练习题2：玩家状态变化系统

**任务：** 创建一个玩家状态系统，当玩家状态改变时通知UI

```csharp
public enum PlayerState
{
    Idle,      // 待机
    Running,   // 奔跑
    Jumping,   // 跳跃
    Attacking  // 攻击
}

public class Player : MonoBehaviour
{
    private PlayerState currentState = PlayerState.Idle;
    
    // TODO: 创建事件 OnStateChanged
    // 参数：PlayerState newState
    
    public void ChangeState(PlayerState newState)
    {
        if(currentState != newState)
        {
            currentState = newState;
            Debug.Log($"状态改变为：{newState}");
            
            // TODO: 触发事件
        }
    }
}

// TODO: 创建 PlayerUI 类
// 订阅 OnStateChanged 事件
// 实现 UpdateStateText 方法，更新状态文本
```

**答案：**

```csharp
public enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Attacking
}

public class Player : MonoBehaviour
{
    private PlayerState currentState = PlayerState.Idle;
    
    // 创建事件
    public event Action<PlayerState> OnStateChanged;
    
    public void ChangeState(PlayerState newState)
    {
        if(currentState != newState)
        {
            currentState = newState;
            Debug.Log($"状态改变为：{newState}");
            
            // 触发事件
            OnStateChanged?.Invoke(newState);
        }
    }
    
    void Update()
    {
        // 示例：根据输入改变状态
        if(Input.GetKey(KeyCode.W))
        {
            ChangeState(PlayerState.Running);
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(PlayerState.Jumping);
        }
        else if(Input.GetMouseButtonDown(0))
        {
            ChangeState(PlayerState.Attacking);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
    }
}

public class PlayerUI : MonoBehaviour
{
    void OnEnable()
    {
        Player player = FindObjectOfType<Player>();
        player.OnStateChanged += UpdateStateText;
    }
    
    void OnDisable()
    {
        Player player = FindObjectOfType<Player>();
        if(player != null)
        {
            player.OnStateChanged -= UpdateStateText;
        }
    }
    
    void UpdateStateText(PlayerState newState)
    {
        string stateText = newState switch
        {
            PlayerState.Idle => "待机中",
            PlayerState.Running => "奔跑中",
            PlayerState.Jumping => "跳跃中",
            PlayerState.Attacking => "攻击中",
            _ => "未知状态"
        };
        
        Debug.Log($"UI更新：{stateText}");
    }
}
```

---

## 3. Lambda表达式

### 什么是Lambda表达式？

Lambda表达式是一种"匿名函数"的简写方式。它让你可以快速创建一个方法，而不需要正式声明它。

**生活中的类比：**
- 传统方法：像是正式雇佣一个员工，需要签合同、办入职
- Lambda表达式：像是临时找个人帮忙，不需要那么多手续

### 为什么需要Lambda表达式？

有时候你只需要一个简单的方法，只用一次，为它专门写一个完整的方法声明太麻烦了。

### 基本语法对比

**传统方法：**

```csharp
public class TraditionalWay : MonoBehaviour
{
    void Start()
    {
        // 需要先声明方法
        Action<string> printAction = PrintMessage;
        printAction("Hello");
    }
    
    // 需要单独写一个方法
    void PrintMessage(string msg)
    {
        Debug.Log(msg);
    }
}
```

**使用Lambda表达式：**

```csharp
public class LambdaWay : MonoBehaviour
{
    void Start()
    {
        // 直接在这里写方法体
        Action<string> printAction = (msg) => Debug.Log(msg);
        printAction("Hello");
    }
}
```

### Lambda表达式的语法详解

#### 基本格式

```
(参数) => 表达式或语句块
```

#### 示例1：无参数

```csharp
// 传统方法
void SayHello()
{
    Debug.Log("Hello");
}
Action action1 = SayHello;

// Lambda表达式
Action action2 = () => Debug.Log("Hello");
//               ^^    ^^^^^^^^^^^^^^^^^^
//               |     表达式（方法体）
//               参数列表（空）
```

**详细解释：**
- `()`：空的参数列表
- `=>`：Lambda运算符，读作"goes to"
- `Debug.Log("Hello")`：要执行的代码

#### 示例2：一个参数

```csharp
// 传统方法
void PrintNumber(int num)
{
    Debug.Log(num);
}
Action<int> action1 = PrintNumber;

// Lambda表达式（完整写法）
Action<int> action2 = (int num) => Debug.Log(num);

// Lambda表达式（简化写法，省略类型）
Action<int> action3 = (num) => Debug.Log(num);

// Lambda表达式（只有一个参数时，可以省略括号）
Action<int> action4 = num => Debug.Log(num);
```

#### 示例3：多个参数

```csharp
// 传统方法
void Add(int a, int b)
{
    Debug.Log(a + b);
}
Action<int, int> action1 = Add;

// Lambda表达式
Action<int, int> action2 = (a, b) => Debug.Log(a + b);
```

#### 示例4：多行代码

```csharp
// 当需要执行多行代码时，使用花括号
Action<string> action = (msg) => 
{
    Debug.Log("开始处理");
    Debug.Log(msg);
    Debug.Log("处理完成");
};

action("Hello");
// 输出：
// 开始处理
// Hello
// 处理完成
```


### Unity实际应用示例

#### 场景1：按钮点击事件

```csharp
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public Button settingsButton;
    
    void Start()
    {
        // 传统方式：需要为每个按钮写一个方法
        // startButton.onClick.AddListener(OnStartButtonClick);
        
        // 使用Lambda：直接在这里写逻辑
        startButton.onClick.AddListener(() => 
        {
            Debug.Log("开始游戏");
            SceneManager.LoadScene("GameScene");
        });
        
        quitButton.onClick.AddListener(() => 
        {
            Debug.Log("退出游戏");
            Application.Quit();
        });
        
        settingsButton.onClick.AddListener(() => 
        {
            Debug.Log("打开设置");
            // 可以访问外部变量
            ShowSettings();
        });
    }
    
    void ShowSettings()
    {
        Debug.Log("显示设置面板");
    }
}
```

**优势：**
- 代码更紧凑，逻辑集中在一起
- 不需要为每个按钮单独写方法
- 可以访问局部变量

#### 场景2：延迟执行

```csharp
public class DelayedAction : MonoBehaviour
{
    void Start()
    {
        int score = 100;
        
        // 使用Lambda可以捕获局部变量
        StartCoroutine(DelayAndExecute(3f, () => 
        {
            Debug.Log($"3秒后执行，分数：{score}");
            score += 50;  // 可以修改外部变量
            Debug.Log($"新分数：{score}");
        }));
    }
    
    IEnumerator DelayAndExecute(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
```

**执行流程：**

```
1. score = 100
2. 启动协程，传入Lambda表达式
3. 等待3秒
4. 执行Lambda表达式：
   → 输出："3秒后执行，分数：100"
   → score = 150
   → 输出："新分数：150"
```

#### 场景3：LINQ查询（高级用法）

```csharp
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    
    void FindEnemies()
    {
        // 查找所有血量低于30的敌人
        var lowHealthEnemies = enemies.Where(e => e.health < 30).ToList();
        //                                    ^^^^^^^^^^^^^^^^^^^
        //                                    Lambda表达式作为筛选条件
        
        Debug.Log($"找到 {lowHealthEnemies.Count} 个低血量敌人");
        
        // 查找距离玩家最近的敌人
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        var nearestEnemy = enemies.OrderBy(e => 
            Vector3.Distance(e.transform.position, player.position)
        ).FirstOrDefault();
        
        if(nearestEnemy != null)
        {
            Debug.Log($"最近的敌人：{nearestEnemy.name}");
        }
        
        // 给所有敌人造成伤害
        enemies.ForEach(e => e.TakeDamage(10));
        //              ^^^^^^^^^^^^^^^^^^^^^^
        //              Lambda表达式作为操作
    }
}
```

**详细解释：**

```csharp
// Where方法：筛选满足条件的元素
enemies.Where(e => e.health < 30)
//            ^    ^^^^^^^^^^^^^
//            |    条件：血量小于30
//            参数：每个敌人

// OrderBy方法：按某个值排序
enemies.OrderBy(e => Vector3.Distance(...))
//              ^    ^^^^^^^^^^^^^^^^^^^^^^
//              |    排序依据：距离
//              参数：每个敌人

// ForEach方法：对每个元素执行操作
enemies.ForEach(e => e.TakeDamage(10))
//              ^    ^^^^^^^^^^^^^^^^^^
//              |    操作：造成10点伤害
//              参数：每个敌人
```

### 练习题1：计时器系统

**任务：** 使用Lambda表达式实现一个灵活的计时器系统

```csharp
public class Timer : MonoBehaviour
{
    // TODO: 创建方法 StartTimer(float duration, Action onComplete)
    // 使用协程实现倒计时
    // 时间到后调用 onComplete
    
    void Start()
    {
        // TODO: 使用Lambda表达式调用计时器
        // 3秒后输出 "时间到！"
        
        // TODO: 再创建一个5秒计时器
        // 时间到后输出 "5秒计时器结束" 和 "获得奖励：100"
    }
}
```

**答案：**

```csharp
public class Timer : MonoBehaviour
{
    public void StartTimer(float duration, Action onComplete)
    {
        StartCoroutine(TimerCoroutine(duration, onComplete));
    }
    
    IEnumerator TimerCoroutine(float duration, Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
    }
    
    void Start()
    {
        // 3秒计时器
        StartTimer(3f, () => 
        {
            Debug.Log("时间到！");
        });
        
        // 5秒计时器（多行代码）
        StartTimer(5f, () => 
        {
            Debug.Log("5秒计时器结束");
            int bonus = 100;
            Debug.Log($"获得奖励：{bonus}");
        });
        
        // 也可以在Lambda中访问外部变量
        int playerLevel = 10;
        StartTimer(10f, () => 
        {
            Debug.Log($"玩家等级：{playerLevel}");
            playerLevel++;
            Debug.Log($"升级到：{playerLevel}");
        });
    }
}
```

### 练习题2：敌人筛选系统

**任务：** 使用Lambda表达式筛选和操作敌人列表

```csharp
using System.Linq;

public class EnemyFilter : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    
    void Start()
    {
        // TODO: 使用Where筛选血量大于50的敌人
        
        // TODO: 使用OrderBy按血量从低到高排序
        
        // TODO: 使用ForEach给所有敌人造成20点伤害
    }
}

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{name} 受到 {damage} 伤害，剩余血量：{health}");
    }
}
```

**答案：**

```csharp
using System.Linq;

public class EnemyFilter : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    
    void Start()
    {
        // 筛选血量大于50的敌人
        var healthyEnemies = enemies.Where(e => e.health > 50).ToList();
        Debug.Log($"血量大于50的敌人数量：{healthyEnemies.Count}");
        
        // 按血量从低到高排序
        var sortedEnemies = enemies.OrderBy(e => e.health).ToList();
        Debug.Log($"血量最低的敌人：{sortedEnemies[0].name}，血量：{sortedEnemies[0].health}");
        
        // 给所有敌人造成20点伤害
        enemies.ForEach(e => e.TakeDamage(20));
        
        // 组合使用：找到血量最低的敌人并造成额外伤害
        var weakestEnemy = enemies.OrderBy(e => e.health).FirstOrDefault();
        if(weakestEnemy != null)
        {
            weakestEnemy.TakeDamage(50);
            Debug.Log($"对最弱的敌人造成额外伤害");
        }
    }
}
```

---

## 4. 协程（Coroutine）

### 什么是协程？

协程是Unity中实现"暂停"和"等待"的机制。它可以让一个方法执行到一半时暂停，等待一段时间或某个条件满足后再继续执行。

**生活中的类比：**
想象你在煮饭：
1. 把米放进电饭煲
2. 按下开关
3. **等待30分钟**（这就是协程的"暂停"）
4. 饭煮好了，继续下一步

在这个过程中，你不需要一直站在电饭煲旁边等，你可以去做其他事情。协程就是这样，让程序可以"等待"而不阻塞其他代码的执行。

### 为什么需要协程？

**问题：如果没有协程**

```csharp
void BadExample()
{
    Debug.Log("开始");
    
    // 想要等待3秒...但这样不行！
    // Thread.Sleep(3000);  // 这会冻结整个游戏！
    
    Debug.Log("3秒后");  // 实际上会立即执行
}
```

**解决：使用协程**

```csharp
IEnumerator GoodExample()
{
    Debug.Log("开始");
    
    yield return new WaitForSeconds(3f);  // 等待3秒，不冻结游戏
    
    Debug.Log("3秒后");  // 真的会在3秒后执行
}
```

### 协程的基本语法

#### 声明协程

```csharp
// 协程方法的返回类型必须是 IEnumerator
IEnumerator MyCoroutine()
{
    Debug.Log("协程开始");
    
    // yield return 表示"暂停并返回"
    yield return new WaitForSeconds(1f);
    
    Debug.Log("1秒后");
}
```

**详细解释：**

- `IEnumerator`：协程的返回类型（必须）
- `yield return`：暂停协程，返回一个值
- `new WaitForSeconds(1f)`：等待1秒

#### 启动协程

```csharp
void Start()
{
    // 方式1：推荐
    StartCoroutine(MyCoroutine());
    
    // 方式2：使用字符串（不推荐，容易出错）
    StartCoroutine("MyCoroutine");
}
```

#### 停止协程

```csharp
private Coroutine myCoroutine;

void Start()
{
    // 保存协程引用
    myCoroutine = StartCoroutine(MyCoroutine());
}

void Update()
{
    if(Input.GetKeyDown(KeyCode.Space))
    {
        // 停止协程
        if(myCoroutine != null)
        {
            StopCoroutine(myCoroutine);
            Debug.Log("协程已停止");
        }
    }
}
```

### yield return 的各种用法

```csharp
IEnumerator YieldExamples()
{
    // 1. 等待指定秒数
    yield return new WaitForSeconds(2f);
    Debug.Log("等待了2秒");
    
    // 2. 等待下一帧
    yield return null;
    Debug.Log("等待了一帧");
    
    // 3. 等待物理更新
    yield return new WaitForFixedUpdate();
    Debug.Log("等待了一次物理更新");
    
    // 4. 等待帧结束
    yield return new WaitForEndOfFrame();
    Debug.Log("等待到帧结束");
    
    // 5. 等待条件满足
    yield return new WaitUntil(() => health <= 0);
    Debug.Log("血量归零了");
    
    // 6. 等待条件不满足
    yield return new WaitWhile(() => isMoving);
    Debug.Log("停止移动了");
    
    // 7. 等待另一个协程完成
    yield return StartCoroutine(OtherCoroutine());
    Debug.Log("另一个协程完成了");
}
```

**详细解释：**

| yield return | 含义 | 使用场景 |
|-------------|------|---------|
| `new WaitForSeconds(n)` | 等待n秒 | 延迟执行、倒计时 |
| `null` | 等待一帧 | 分帧处理、动画 |
| `new WaitForFixedUpdate()` | 等待物理更新 | 物理相关操作 |
| `new WaitForEndOfFrame()` | 等待帧结束 | 截图、渲染相关 |
| `new WaitUntil(条件)` | 等待条件为true | 等待某个状态 |
| `new WaitWhile(条件)` | 等待条件为false | 等待某个状态结束 |

### Unity实际应用示例

#### 场景1：淡入淡出效果

```csharp
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public Image fadeImage;
    
    void Start()
    {
        // 游戏开始时淡入
        StartCoroutine(FadeIn(2f));
    }
    
    // 淡入：从黑色渐变到透明
    IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0f;  // 已经过的时间
        Color color = fadeImage.color;
        
        // 循环执行，直到时间到
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;  // 累加时间
            
            // 计算当前的透明度（从1到0）
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            color.a = alpha;
            fadeImage.color = color;
            
            yield return null;  // 等待下一帧，形成动画效果
        }
        
        // 确保最终值准确
        color.a = 0f;
        fadeImage.color = color;
        
        Debug.Log("淡入完成");
    }
    
    // 淡出：从透明渐变到黑色
    IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // 计算当前的透明度（从0到1）
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            color.a = alpha;
            fadeImage.color = color;
            
            yield return null;
        }
        
        color.a = 1f;
        fadeImage.color = color;
        
        Debug.Log("淡出完成");
    }
}
```

**执行流程详解：**

```
FadeIn(2f) 开始
↓
elapsedTime = 0, alpha = 1.0 (完全不透明)
↓
yield return null (等待一帧，约0.016秒)
↓
elapsedTime = 0.016, alpha = 0.992
↓
yield return null
↓
elapsedTime = 0.032, alpha = 0.984
↓
... (重复约120帧)
↓
elapsedTime = 2.0, alpha = 0.0 (完全透明)
↓
协程结束
```


#### 场景2：倒计时系统

```csharp
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public Text countdownText;
    
    void Start()
    {
        StartCoroutine(CountdownCoroutine(10));
    }
    
    IEnumerator CountdownCoroutine(int seconds)
    {
        int remaining = seconds;
        
        while(remaining > 0)
        {
            // 更新UI
            countdownText.text = remaining.ToString();
            Debug.Log($"倒计时：{remaining}");
            
            // 等待1秒
            yield return new WaitForSeconds(1f);
            
            // 减少剩余时间
            remaining--;
        }
        
        // 倒计时结束
        countdownText.text = "开始！";
        Debug.Log("倒计时结束，游戏开始");
    }
}
```

**执行流程：**

```
CountdownCoroutine(10) 开始
↓
remaining = 10
显示 "10"
↓
yield return new WaitForSeconds(1f) (等待1秒)
↓
remaining = 9
显示 "9"
↓
yield return new WaitForSeconds(1f)
↓
... (重复)
↓
remaining = 1
显示 "1"
↓
yield return new WaitForSeconds(1f)
↓
remaining = 0
显示 "开始！"
↓
协程结束
```

#### 场景3：分帧加载（避免卡顿）

```csharp
public class LevelLoader : MonoBehaviour
{
    public List<GameObject> objectsToLoad;
    
    void Start()
    {
        StartCoroutine(LoadObjectsOverTime());
    }
    
    IEnumerator LoadObjectsOverTime()
    {
        Debug.Log("开始加载关卡");
        
        int count = 0;
        foreach(GameObject obj in objectsToLoad)
        {
            // 实例化对象
            Instantiate(obj);
            count++;
            Debug.Log($"加载了 {obj.name} ({count}/{objectsToLoad.Count})");
            
            // 每加载一个对象等待一帧
            // 这样可以避免一次性加载太多导致游戏卡顿
            yield return null;
        }
        
        Debug.Log("关卡加载完成");
    }
}
```

**为什么要分帧加载？**

```
不使用协程（一次性加载）：
Frame 1: 加载100个对象 → 游戏卡顿！
Frame 2: 正常运行

使用协程（分帧加载）：
Frame 1: 加载1个对象 → 流畅
Frame 2: 加载1个对象 → 流畅
Frame 3: 加载1个对象 → 流畅
...
Frame 100: 加载1个对象 → 流畅
```

#### 场景4：重复执行的协程

```csharp
public class RepeatingCoroutine : MonoBehaviour
{
    private Coroutine spawnCoroutine;
    
    void Start()
    {
        // 启动重复生成敌人的协程
        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }
    
    IEnumerator SpawnEnemies()
    {
        while(true)  // 无限循环
        {
            Debug.Log("生成一个敌人");
            // 这里可以实例化敌人
            
            // 等待2秒后再生成下一个
            yield return new WaitForSeconds(2f);
        }
    }
    
    void Update()
    {
        // 按空格键停止生成
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                Debug.Log("停止生成敌人");
            }
        }
    }
}
```

**执行流程：**

```
SpawnEnemies() 开始
↓
输出："生成一个敌人"
↓
yield return new WaitForSeconds(2f) (等待2秒)
↓
输出："生成一个敌人"
↓
yield return new WaitForSeconds(2f)
↓
... (无限重复，直到被停止)
```

### 协程的常见陷阱

#### 陷阱1：在OnDisable中协程会自动停止

```csharp
public class CoroutineTrap : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LongRunningCoroutine());
    }
    
    IEnumerator LongRunningCoroutine()
    {
        Debug.Log("协程开始");
        yield return new WaitForSeconds(5f);
        Debug.Log("5秒后");  // 如果GameObject被禁用，这行不会执行
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            gameObject.SetActive(false);  // 禁用GameObject会停止所有协程
        }
    }
}
```

#### 陷阱2：协程不能有返回值

```csharp
// 错误！协程不能返回具体的值
IEnumerator GetScore()
{
    yield return new WaitForSeconds(1f);
    return 100;  // 编译错误！
}

// 正确：使用回调或事件
IEnumerator GetScore(Action<int> callback)
{
    yield return new WaitForSeconds(1f);
    callback(100);  // 通过回调返回值
}

void Start()
{
    StartCoroutine(GetScore((score) => 
    {
        Debug.Log($"获得分数：{score}");
    }));
}
```

### 练习题1：技能冷却系统

**任务：** 创建一个技能系统，使用协程实现冷却机制

```csharp
public class Skill : MonoBehaviour
{
    public string skillName = "火球术";
    public float cooldownTime = 5f;
    private bool isOnCooldown = false;
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
        }
    }
    
    void UseSkill()
    {
        if(isOnCooldown)
        {
            Debug.Log($"{skillName} 冷却中");
            return;
        }
        
        Debug.Log($"使用技能：{skillName}");
        
        // TODO: 启动冷却协程
    }
    
    // TODO: 创建冷却协程 CooldownCoroutine
    // 1. 设置 isOnCooldown = true
    // 2. 输出 "开始冷却"
    // 3. 等待 cooldownTime 秒
    // 4. 设置 isOnCooldown = false
    // 5. 输出 "冷却完成"
}
```

**答案：**

```csharp
public class Skill : MonoBehaviour
{
    public string skillName = "火球术";
    public float cooldownTime = 5f;
    private bool isOnCooldown = false;
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
        }
    }
    
    void UseSkill()
    {
        if(isOnCooldown)
        {
            Debug.Log($"{skillName} 冷却中");
            return;
        }
        
        Debug.Log($"使用技能：{skillName}");
        StartCoroutine(CooldownCoroutine());
    }
    
    IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true;
        Debug.Log($"{skillName} 开始冷却");
        
        yield return new WaitForSeconds(cooldownTime);
        
        isOnCooldown = false;
        Debug.Log($"{skillName} 冷却完成");
    }
}
```

**执行流程：**

```
按下Q键
↓
UseSkill() 被调用
↓
isOnCooldown = false，可以使用
↓
输出："使用技能：火球术"
↓
启动 CooldownCoroutine()
↓
isOnCooldown = true
输出："火球术 开始冷却"
↓
yield return new WaitForSeconds(5f) (等待5秒)
↓
isOnCooldown = false
输出："火球术 冷却完成"
↓
协程结束

如果在冷却期间再按Q键：
↓
UseSkill() 被调用
↓
isOnCooldown = true，不能使用
↓
输出："火球术 冷却中"
↓
直接返回
```

### 练习题2：生命恢复系统

**任务：** 创建一个生命恢复系统，每秒恢复一定血量

```csharp
public class HealthRegeneration : MonoBehaviour
{
    public float health = 50f;
    public float maxHealth = 100f;
    public float regenAmount = 5f;  // 每秒恢复量
    private Coroutine regenCoroutine;
    
    void Start()
    {
        // TODO: 启动恢复协程
    }
    
    // TODO: 创建恢复协程 RegenerateHealth
    // 1. 使用 while(true) 无限循环
    // 2. 如果 health < maxHealth，增加 regenAmount
    // 3. 确保 health 不超过 maxHealth
    // 4. 输出当前血量
    // 5. 等待1秒
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health < 0) health = 0;
        Debug.Log($"受到伤害，当前血量：{health}");
    }
}
```

**答案：**

```csharp
public class HealthRegeneration : MonoBehaviour
{
    public float health = 50f;
    public float maxHealth = 100f;
    public float regenAmount = 5f;
    private Coroutine regenCoroutine;
    
    void Start()
    {
        regenCoroutine = StartCoroutine(RegenerateHealth());
    }
    
    IEnumerator RegenerateHealth()
    {
        while(true)
        {
            // 如果血量未满，恢复血量
            if(health < maxHealth)
            {
                health += regenAmount;
                
                // 确保不超过最大值
                if(health > maxHealth)
                {
                    health = maxHealth;
                }
                
                Debug.Log($"恢复生命，当前血量：{health}/{maxHealth}");
            }
            
            // 等待1秒后继续
            yield return new WaitForSeconds(1f);
        }
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health < 0) health = 0;
        Debug.Log($"受到伤害，当前血量：{health}");
    }
    
    void Update()
    {
        // 测试：按空格键受到伤害
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }
}
```

**执行流程：**

```
游戏开始，health = 50
↓
RegenerateHealth() 启动
↓
health < maxHealth，恢复5点
health = 55
输出："恢复生命，当前血量：55/100"
↓
yield return new WaitForSeconds(1f) (等待1秒)
↓
health = 60
输出："恢复生命，当前血量：60/100"
↓
... (持续恢复)
↓
health = 100 (满血)
↓
不再恢复，但协程继续运行
↓
yield return new WaitForSeconds(1f)
↓
检查 health < maxHealth (false)
↓
... (循环等待)

如果按空格键受到伤害：
↓
health = 80
输出："受到伤害，当前血量：80"
↓
协程继续运行
↓
health = 85
输出："恢复生命，当前血量：85/100"
↓
... (继续恢复到满血)
```

---

## 总结

### 上篇内容回顾

我们学习了4个核心概念：

1. **委托（Delegate）**
   - 方法的容器，可以存储方法引用
   - 支持多播（一次调用多个方法）
   - 用于回调和事件系统的基础

2. **事件（Event）**
   - 基于委托，提供更好的封装
   - 外部只能订阅/取消订阅，不能直接调用
   - 使用Action简化声明

3. **Lambda表达式**
   - 匿名函数的简写
   - 语法：`(参数) => 表达式`
   - 常用于简短的回调和LINQ查询

4. **协程（Coroutine）**
   - 实现暂停和等待的机制
   - 返回类型必须是IEnumerator
   - 使用yield return暂停执行

### 下篇预告

下篇将学习：
- 泛型（Generic）
- 空值条件运算符（?.）
- 属性（Property）
- 接口（Interface）

---

*继续学习请查看《C#核心概念-下篇.md》*
