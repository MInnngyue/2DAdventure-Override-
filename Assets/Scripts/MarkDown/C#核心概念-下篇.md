# C#核心概念 - Unity开发必备（下篇）

## 目录

5. [泛型（Generic）](#5-泛型generic)
6. [空值条件运算符（?.）](#6-空值条件运算符)
7. [属性（Property）](#7-属性property)
8. [接口（Interface）](#8-接口interface)

---

## 5. 泛型（Generic）

### 什么是泛型？

泛型允许你编写可以处理多种类型的代码，而不需要为每种类型都写一遍。使用`<T>`表示类型参数。

**生活中的类比：**
想象你有一个万能盒子：
- 可以装苹果
- 可以装书
- 可以装玩具

你不需要为每种物品制作不同的盒子，一个万能盒子就够了。泛型就是这样的"万能盒子"。

### 为什么需要泛型？

**问题：没有泛型时**

```csharp
// 需要为每种类型写一个类
public class IntContainer
{
    public int value;
    
    public void SetValue(int newValue)
    {
        value = newValue;
    }
    
    public int GetValue()
    {
        return value;
    }
}

public class StringContainer
{
    public string value;
    
    public void SetValue(string newValue)
    {
        value = newValue;
    }
    
    public string GetValue()
    {
        return value;
    }
}

public class FloatContainer
{
    public float value;
    // ... 又要写一遍相同的代码
}
```

**解决：使用泛型**

```csharp
// 一个类就够了！
public class Container<T>
{
    public T value;
    
    public void SetValue(T newValue)
    {
        value = newValue;
    }
    
    public T GetValue()
    {
        return value;
    }
}

// 使用
Container<int> intContainer = new Container<int>();
intContainer.SetValue(100);

Container<string> stringContainer = new Container<string>();
stringContainer.SetValue("Hello");

Container<float> floatContainer = new Container<float>();
floatContainer.SetValue(3.14f);
```

### 泛型的基本语法详解

#### 声明泛型类

```csharp
public class Box<T>
//            ^^^
//            泛型类型参数，T是占位符
{
    private T item;
    //      ^
    //      T可以在类中任何地方使用
    
    public void Put(T newItem)
    //              ^
    //              方法参数也可以使用T
    {
        item = newItem;
    }
    
    public T Get()
    //     ^
    //     返回类型也可以使用T
    {
        return item;
    }
}
```

**详细解释：**

1. `<T>`：声明这是一个泛型类，T是类型参数的名字（可以随便起名，但习惯用T）
2. `T item`：T会被替换成实际的类型
3. 当你创建`Box<int>`时，所有的T都会变成int
4. 当你创建`Box<string>`时，所有的T都会变成string

**使用示例：**

```csharp
public class GenericExample : MonoBehaviour
{
    void Start()
    {
        // 创建一个装int的盒子
        Box<int> intBox = new Box<int>();
        intBox.Put(42);
        int number = intBox.Get();
        Debug.Log($"从盒子里取出：{number}");  // 输出：42
        
        // 创建一个装string的盒子
        Box<string> stringBox = new Box<string>();
        stringBox.Put("Hello");
        string text = stringBox.Get();
        Debug.Log($"从盒子里取出：{text}");  // 输出：Hello
        
        // 创建一个装GameObject的盒子
        Box<GameObject> objectBox = new Box<GameObject>();
        objectBox.Put(gameObject);
        GameObject obj = objectBox.Get();
        Debug.Log($"从盒子里取出：{obj.name}");
    }
}
```

**执行流程：**

```
Box<int> intBox = new Box<int>();
↓
编译器将所有T替换为int：
class Box<int>
{
    private int item;
    public void Put(int newItem) { item = newItem; }
    public int Get() { return item; }
}
↓
intBox.Put(42)
↓
item = 42
↓
intBox.Get()
↓
return 42
```


### 泛型约束（where关键字）

有时候你需要限制泛型类型，确保它具有某些特性：

```csharp
// 约束1：T必须是类（引用类型）
public class ClassOnly<T> where T : class
{
    public T value;
}

// 约束2：T必须是结构体（值类型）
public class StructOnly<T> where T : struct
{
    public T value;
}

// 约束3：T必须继承自MonoBehaviour
public class ComponentOnly<T> where T : MonoBehaviour
{
    public T component;
    
    public void DoSomething()
    {
        // 因为约束了T必须是MonoBehaviour
        // 所以可以调用MonoBehaviour的方法
        component.gameObject.SetActive(true);
    }
}

// 约束4：T必须有无参构造函数
public class NewableOnly<T> where T : new()
{
    public T CreateNew()
    {
        return new T();  // 可以创建T的实例
    }
}

// 约束5：多个约束
public class Multiple<T> where T : MonoBehaviour, IComparable, new()
{
    // T必须：
    // 1. 继承自MonoBehaviour
    // 2. 实现IComparable接口
    // 3. 有无参构造函数
}
```

**详细解释：**

```csharp
// 示例：为什么需要约束
public class Enemy : MonoBehaviour
{
    public int health = 100;
}

public class EnemyManager<T> where T : Enemy
//                           ^^^^^^^^^^^^^^^
//                           约束：T必须是Enemy或Enemy的子类
{
    public void DamageEnemy(T enemy)
    {
        // 因为约束了T必须是Enemy
        // 所以可以访问Enemy的成员
        enemy.health -= 10;
        Debug.Log($"{enemy.name} 受到伤害，剩余血量：{enemy.health}");
    }
}
```

### Unity实际应用：对象池系统

对象池是泛型的经典应用，可以复用对象，提高性能：

```csharp
public class ObjectPool<T> where T : MonoBehaviour
{
    private T prefab;                    // 预制体
    private List<T> pool = new List<T>();  // 对象池
    
    // 构造函数：初始化对象池
    public ObjectPool(T prefab, int initialSize)
    {
        this.prefab = prefab;
        
        // 预先创建一些对象
        for(int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(prefab);
            obj.gameObject.SetActive(false);  // 禁用对象
            pool.Add(obj);
        }
        
        Debug.Log($"创建了 {initialSize} 个 {typeof(T).Name} 对象");
    }
    
    // 从池中获取对象
    public T Get()
    {
        // 遍历池，找到未激活的对象
        foreach(T obj in pool)
        {
            if(!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }
        
        // 如果池中没有可用对象，创建新的
        T newObj = GameObject.Instantiate(prefab);
        pool.Add(newObj);
        Debug.Log($"池已满，创建新对象，当前池大小：{pool.Count}");
        return newObj;
    }
    
    // 归还对象到池中
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
    }
    
    // 获取池的大小
    public int GetPoolSize()
    {
        return pool.Count;
    }
}

// 使用示例
public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

public class BulletManager : MonoBehaviour
{
    public Bullet bulletPrefab;
    private ObjectPool<Bullet> bulletPool;
    
    void Start()
    {
        // 创建子弹对象池，初始大小20
        bulletPool = new ObjectPool<Bullet>(bulletPrefab, 20);
    }
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    
    void Shoot()
    {
        // 从池中获取子弹
        Bullet bullet = bulletPool.Get();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        
        Debug.Log("发射子弹");
        
        // 3秒后归还子弹
        StartCoroutine(ReturnBulletAfterDelay(bullet, 3f));
    }
    
    IEnumerator ReturnBulletAfterDelay(Bullet bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bulletPool.Return(bullet);
    }
}
```

**执行流程详解：**

```
1. 初始化
   ObjectPool<Bullet> bulletPool = new ObjectPool<Bullet>(bulletPrefab, 20)
   ↓
   创建20个Bullet对象，全部禁用
   pool = [Bullet1(禁用), Bullet2(禁用), ..., Bullet20(禁用)]

2. 第一次射击
   bulletPool.Get()
   ↓
   遍历pool，找到第一个禁用的对象：Bullet1
   ↓
   激活Bullet1
   ↓
   返回Bullet1

3. 第二次射击
   bulletPool.Get()
   ↓
   遍历pool，找到第一个禁用的对象：Bullet2
   ↓
   激活Bullet2
   ↓
   返回Bullet2

4. 3秒后
   bulletPool.Return(Bullet1)
   ↓
   禁用Bullet1
   ↓
   Bullet1回到池中，可以再次使用

5. 如果池中所有对象都在使用
   bulletPool.Get()
   ↓
   遍历pool，没有找到禁用的对象
   ↓
   创建新的Bullet21
   ↓
   pool.Add(Bullet21)
   ↓
   返回Bullet21
```

### Unity实际应用：单例模式

```csharp
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                // 尝试在场景中查找
                instance = FindObjectOfType<T>();
                
                if(instance == null)
                {
                    // 如果找不到，创建新的
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }
    
    protected virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

// 使用示例
public class GameManager : Singleton<GameManager>
{
    public int score = 0;
    
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"分数：{score}");
    }
}

public class AudioManager : Singleton<AudioManager>
{
    public void PlaySound(string soundName)
    {
        Debug.Log($"播放音效：{soundName}");
    }
}

// 在其他脚本中使用
public class Player : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.AddScore(10);
            AudioManager.Instance.PlaySound("coin");
        }
    }
}
```

**详细解释：**

```
Singleton<GameManager>
↓
T = GameManager
↓
class Singleton<GameManager>
{
    private static GameManager instance;
    public static GameManager Instance { get { ... } }
}
↓
GameManager.Instance
↓
如果instance为null，查找或创建GameManager
↓
返回GameManager实例
```

### 泛型方法

除了泛型类，还可以创建泛型方法：

```csharp
public class ComponentHelper : MonoBehaviour
{
    // 泛型方法：获取或添加组件
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    //                                 ^
    //                                 方法级别的泛型参数
    {
        T component = obj.GetComponent<T>();
        if(component == null)
        {
            component = obj.AddComponent<T>();
            Debug.Log($"添加了组件：{typeof(T).Name}");
        }
        return component;
    }
    
    // 使用示例
    void Start()
    {
        Rigidbody2D rb = GetOrAddComponent<Rigidbody2D>(gameObject);
        BoxCollider2D collider = GetOrAddComponent<BoxCollider2D>(gameObject);
        AudioSource audio = GetOrAddComponent<AudioSource>(gameObject);
    }
}
```

**执行流程：**

```
GetOrAddComponent<Rigidbody2D>(gameObject)
↓
T = Rigidbody2D
↓
尝试获取Rigidbody2D组件
↓
如果没有，添加Rigidbody2D组件
↓
返回Rigidbody2D组件
```

### 练习题1：泛型数据管理器

**任务：** 创建一个泛型数据管理器，可以管理任何类型的数据

```csharp
// TODO: 创建泛型类 DataManager<T>
// 功能：
// 1. 使用 List<T> 存储数据
// 2. Add(T data) - 添加数据
// 3. Remove(T data) - 移除数据
// 4. GetAll() - 获取所有数据
// 5. Clear() - 清空数据
// 6. Count - 属性，返回数据数量

[System.Serializable]
public class PlayerData
{
    public string name;
    public int level;
    public float health;
}

// 使用示例：
// DataManager<PlayerData> playerManager = new DataManager<PlayerData>();
// DataManager<int> scoreManager = new DataManager<int>();
```

**答案：**

```csharp
public class DataManager<T>
{
    private List<T> dataList = new List<T>();
    
    // 添加数据
    public void Add(T data)
    {
        dataList.Add(data);
        Debug.Log($"添加数据，当前数量：{dataList.Count}");
    }
    
    // 移除数据
    public void Remove(T data)
    {
        if(dataList.Remove(data))
        {
            Debug.Log($"移除数据，当前数量：{dataList.Count}");
        }
        else
        {
            Debug.Log("数据不存在");
        }
    }
    
    // 获取所有数据
    public List<T> GetAll()
    {
        return new List<T>(dataList);  // 返回副本，防止外部修改
    }
    
    // 清空数据
    public void Clear()
    {
        dataList.Clear();
        Debug.Log("数据已清空");
    }
    
    // 数据数量
    public int Count
    {
        get { return dataList.Count; }
    }
}

// 使用示例
[System.Serializable]
public class PlayerData
{
    public string name;
    public int level;
    public float health;
    
    public PlayerData(string name, int level, float health)
    {
        this.name = name;
        this.level = level;
        this.health = health;
    }
}

public class DataManagerExample : MonoBehaviour
{
    void Start()
    {
        // 管理玩家数据
        DataManager<PlayerData> playerManager = new DataManager<PlayerData>();
        
        PlayerData player1 = new PlayerData("玩家1", 10, 100);
        PlayerData player2 = new PlayerData("玩家2", 5, 80);
        
        playerManager.Add(player1);
        playerManager.Add(player2);
        
        Debug.Log($"玩家数量：{playerManager.Count}");
        
        List<PlayerData> allPlayers = playerManager.GetAll();
        foreach(PlayerData player in allPlayers)
        {
            Debug.Log($"{player.name} - 等级{player.level} - 血量{player.health}");
        }
        
        // 管理分数
        DataManager<int> scoreManager = new DataManager<int>();
        scoreManager.Add(100);
        scoreManager.Add(200);
        scoreManager.Add(300);
        
        Debug.Log($"分数记录数量：{scoreManager.Count}");
    }
}
```

### 练习题2：泛型对象查找器

**任务：** 创建一个泛型工具类，用于查找场景中的对象

```csharp
// TODO: 创建泛型类 ObjectFinder<T> where T : MonoBehaviour
// 功能：
// 1. FindAll() - 查找所有T类型的对象
// 2. FindFirst() - 查找第一个T类型的对象
// 3. FindByName(string name) - 按名字查找T类型的对象
// 4. Count() - 返回T类型对象的数量
```

**答案：**

```csharp
public class ObjectFinder<T> where T : MonoBehaviour
{
    // 查找所有T类型的对象
    public List<T> FindAll()
    {
        T[] objects = GameObject.FindObjectsOfType<T>();
        Debug.Log($"找到 {objects.Length} 个 {typeof(T).Name} 对象");
        return new List<T>(objects);
    }
    
    // 查找第一个T类型的对象
    public T FindFirst()
    {
        T obj = GameObject.FindObjectOfType<T>();
        if(obj != null)
        {
            Debug.Log($"找到 {typeof(T).Name}：{obj.name}");
        }
        else
        {
            Debug.Log($"未找到 {typeof(T).Name}");
        }
        return obj;
    }
    
    // 按名字查找T类型的对象
    public T FindByName(string name)
    {
        T[] objects = GameObject.FindObjectsOfType<T>();
        foreach(T obj in objects)
        {
            if(obj.name == name)
            {
                Debug.Log($"找到 {typeof(T).Name}：{name}");
                return obj;
            }
        }
        Debug.Log($"未找到名为 {name} 的 {typeof(T).Name}");
        return null;
    }
    
    // 返回T类型对象的数量
    public int Count()
    {
        T[] objects = GameObject.FindObjectsOfType<T>();
        return objects.Length;
    }
}

// 使用示例
public class ObjectFinderExample : MonoBehaviour
{
    void Start()
    {
        // 查找所有Enemy
        ObjectFinder<Enemy> enemyFinder = new ObjectFinder<Enemy>();
        List<Enemy> allEnemies = enemyFinder.FindAll();
        Debug.Log($"敌人数量：{enemyFinder.Count()}");
        
        // 查找第一个Player
        ObjectFinder<Player> playerFinder = new ObjectFinder<Player>();
        Player player = playerFinder.FindFirst();
        
        // 按名字查找
        Enemy boss = enemyFinder.FindByName("Boss");
    }
}
```

---

*由于篇幅限制，下篇内容将继续包含：空值条件运算符、属性和接口的详细讲解。*


## 6. 空值条件运算符（?.）

### 什么是空值条件运算符？

空值条件运算符`?.`用于安全地访问可能为null的对象成员。如果对象为null,表达式会返回null而不是抛出异常。

**生活中的类比：**
想象你要敲朋友家的门：
- 传统方式：先确认朋友在家吗？在家才敲门，不在家就不敲
- 空值条件运算符：直接敲门，如果没人就算了，不会出错

### 为什么需要空值条件运算符？

**问题：传统的null检查**

```csharp
public class Player : MonoBehaviour
{
    public Weapon weapon;
    
    void Attack()
    {
        // 传统方式：必须先检查weapon是否为null
        if(weapon != null)
        {
            weapon.Fire();
        }
        
        // 如果不检查，weapon为null时会报错
        // weapon.Fire();  // NullReferenceException!
    }
}
```

**解决：使用空值条件运算符**

```csharp
public class Player : MonoBehaviour
{
    public Weapon weapon;
    
    void Attack()
    {
        // 使用?.运算符，一行搞定
        weapon?.Fire();
        // 如果weapon为null，什么都不做
        // 如果weapon不为null，调用Fire()
    }
}
```

### 基本语法详解

#### 访问成员

```csharp
public class Enemy : MonoBehaviour
{
    public Transform target;
    
    void Update()
    {
        // 传统方式
        if(target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
        }
        
        // 使用?.运算符
        float? distance = Vector3.Distance(transform.position, target?.position ?? Vector3.zero);
        //                                                      ^^
        //                                                      如果target为null，返回null
        //                                                                        ^^^^^^^^^^
        //                                                                        如果为null，使用Vector3.zero
    }
}
```

**详细解释：**

```csharp
target?.position
//    ^^
//    空值条件运算符

执行流程：
1. 检查target是否为null
2. 如果target为null，整个表达式返回null
3. 如果target不为null，返回target.position
```

#### 调用方法

```csharp
public class GameManager : MonoBehaviour
{
    public AudioSource audioSource;
    
    void PlaySound()
    {
        // 传统方式
        if(audioSource != null)
        {
            audioSource.Play();
        }
        
        // 使用?.运算符
        audioSource?.Play();
        //         ^^
        //         如果audioSource为null，不调用Play()
        //         如果audioSource不为null，调用Play()
    }
}
```

#### 链式调用

```csharp
public class Player : MonoBehaviour
{
    public Transform hand;
    
    void CheckWeapon()
    {
        // 传统方式：多层null检查
        if(hand != null)
        {
            Transform weapon = hand.Find("Weapon");
            if(weapon != null)
            {
                Renderer renderer = weapon.GetComponent<Renderer>();
                if(renderer != null)
                {
                    renderer.enabled = false;
                }
            }
        }
        
        // 使用?.运算符：链式调用
        hand?.Find("Weapon")?.GetComponent<Renderer>()?.enabled = false;
        //  ^^             ^^                         ^^
        //  每一步都会检查是否为null
    }
}
```

**执行流程详解：**

```
hand?.Find("Weapon")?.GetComponent<Renderer>()?.enabled = false;

步骤1：检查hand
↓
hand为null？
├─ 是 → 整个表达式结束，什么都不做
└─ 否 → 继续

步骤2：调用hand.Find("Weapon")
↓
返回值为null？
├─ 是 → 整个表达式结束，什么都不做
└─ 否 → 继续

步骤3：调用GetComponent<Renderer>()
↓
返回值为null？
├─ 是 → 整个表达式结束，什么都不做
└─ 否 → 继续

步骤4：设置enabled = false
```

### 空值合并运算符（??）

空值条件运算符经常和空值合并运算符`??`一起使用：

```csharp
public class Player : MonoBehaviour
{
    public string playerName;
    
    void Start()
    {
        // ??运算符：如果左边为null，使用右边的值
        string name = playerName ?? "未命名玩家";
        //                       ^^
        //                       如果playerName为null，使用"未命名玩家"
        
        Debug.Log($"玩家名字：{name}");
    }
}
```

**详细解释：**

```csharp
string name = playerName ?? "未命名玩家";

执行流程：
1. 检查playerName是否为null
2. 如果playerName为null，name = "未命名玩家"
3. 如果playerName不为null，name = playerName
```

### 空值合并赋值运算符（??=）

C# 8.0引入的新运算符，用于简化赋值操作：

```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    public static GameManager Instance
    {
        get
        {
            // 传统方式
            if(instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
            
            // 使用??=运算符
            instance ??= FindObjectOfType<GameManager>();
            //       ^^^
            //       如果instance为null，执行赋值
            //       如果instance不为null，什么都不做
            return instance;
        }
    }
}
```

**详细解释：**

```csharp
instance ??= FindObjectOfType<GameManager>();

等价于：
if(instance == null)
{
    instance = FindObjectOfType<GameManager>();
}
```

### Unity实际应用：安全访问组件

```csharp
public class SafeComponentAccess : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    
    void Start()
    {
        // 获取组件
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        // 安全地访问组件
        // 如果组件不存在，不会报错
        
        // 设置速度
        if(Input.GetKey(KeyCode.D))
        {
            rb?.velocity = new Vector2(5, rb.velocity.y);
            //^^
            //如果rb为null，不执行
        }
        
        // 播放动画
        animator?.SetBool("isRunning", true);
        //       ^^
        //       如果animator为null，不执行
        
        // 播放音效
        if(Input.GetKeyDown(KeyCode.Space))
        {
            audioSource?.Play();
            //         ^^
            //         如果audioSource为null，不执行
        }
    }
    
    // 获取组件的速度，如果组件不存在返回0
    public float GetSpeed()
    {
        return rb?.velocity.magnitude ?? 0f;
        //     ^^                     ^^
        //     如果rb为null，返回null
        //                            如果为null，返回0f
    }
}
```

**执行流程：**

```
rb?.velocity = new Vector2(5, rb.velocity.y);

情况1：rb不为null
↓
rb.velocity = new Vector2(5, rb.velocity.y)
↓
设置速度成功

情况2：rb为null
↓
整个表达式结束
↓
什么都不做，不会报错
```

### Unity实际应用：安全销毁对象

```csharp
public class ObjectDestroyer : MonoBehaviour
{
    public GameObject targetObject;
    
    void DestroyTarget()
    {
        // 传统方式
        if(targetObject != null)
        {
            Destroy(targetObject);
            targetObject = null;
        }
        
        // 使用?.运算符
        Destroy(targetObject);
        targetObject = null;
        
        // 更安全的方式：检查并销毁
        if(targetObject != null)
        {
            Destroy(targetObject);
        }
    }
    
    // 安全获取对象的名字
    public string GetTargetName()
    {
        return targetObject?.name ?? "无目标";
        //                  ^^    ^^
        //                  如果targetObject为null，返回null
        //                        如果为null，返回"无目标"
    }
    
    // 安全获取对象的位置
    public Vector3 GetTargetPosition()
    {
        return targetObject?.transform.position ?? Vector3.zero;
        //                  ^^                   ^^
        //                  如果targetObject为null，返回null
        //                                        如果为null，返回Vector3.zero
    }
}
```

### Unity实际应用：事件系统

```csharp
public class EventManager : MonoBehaviour
{
    // 定义事件
    public event System.Action OnGameStart;
    public event System.Action<int> OnScoreChanged;
    public event System.Action<string> OnPlayerDied;
    
    // 触发事件（安全调用）
    public void StartGame()
    {
        Debug.Log("游戏开始");
        
        // 传统方式
        if(OnGameStart != null)
        {
            OnGameStart();
        }
        
        // 使用?.运算符
        OnGameStart?.Invoke();
        //         ^^
        //         如果OnGameStart为null（没有订阅者），不调用
        //         如果OnGameStart不为null，调用所有订阅者
    }
    
    public void ChangeScore(int newScore)
    {
        Debug.Log($"分数改变：{newScore}");
        OnScoreChanged?.Invoke(newScore);
        //            ^^
        //            安全调用，即使没有订阅者也不会报错
    }
    
    public void PlayerDie(string playerName)
    {
        Debug.Log($"{playerName} 死亡");
        OnPlayerDied?.Invoke(playerName);
    }
}

// 使用示例
public class GameController : MonoBehaviour
{
    private EventManager eventManager;
    
    void Start()
    {
        eventManager = GetComponent<EventManager>();
        
        // 订阅事件
        eventManager.OnGameStart += HandleGameStart;
        eventManager.OnScoreChanged += HandleScoreChanged;
        eventManager.OnPlayerDied += HandlePlayerDied;
    }
    
    void HandleGameStart()
    {
        Debug.Log("处理游戏开始");
    }
    
    void HandleScoreChanged(int score)
    {
        Debug.Log($"处理分数改变：{score}");
    }
    
    void HandlePlayerDied(string playerName)
    {
        Debug.Log($"处理玩家死亡：{playerName}");
    }
    
    void OnDestroy()
    {
        // 取消订阅（安全）
        if(eventManager != null)
        {
            eventManager.OnGameStart -= HandleGameStart;
            eventManager.OnScoreChanged -= HandleScoreChanged;
            eventManager.OnPlayerDied -= HandlePlayerDied;
        }
    }
}
```

**执行流程：**

```
OnGameStart?.Invoke();

情况1：OnGameStart有订阅者
↓
OnGameStart不为null
↓
调用Invoke()
↓
所有订阅的方法被调用

情况2：OnGameStart没有订阅者
↓
OnGameStart为null
↓
整个表达式结束
↓
什么都不做，不会报错
```

### 练习题1：安全的UI更新

**任务：** 创建一个UI管理器，安全地更新UI元素

```csharp
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text healthText;
    public Image healthBar;
    public GameObject gameOverPanel;
    
    // TODO: 实现以下方法，使用?.运算符确保安全
    // 1. UpdateScore(int score) - 更新分数文本
    // 2. UpdateHealth(float health, float maxHealth) - 更新血量文本和血条
    // 3. ShowGameOver() - 显示游戏结束面板
    // 4. HideGameOver() - 隐藏游戏结束面板
    
    // 要求：所有方法都要使用?.运算符，确保UI元素为null时不会报错
}
```

**答案：**

```csharp
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text healthText;
    public Image healthBar;
    public GameObject gameOverPanel;
    
    // 更新分数文本
    public void UpdateScore(int score)
    {
        // 使用?.运算符安全更新
        scoreText?.text = $"分数：{score}";
        //       ^^
        //       如果scoreText为null，不执行
        
        Debug.Log($"更新分数：{score}");
    }
    
    // 更新血量文本和血条
    public void UpdateHealth(float health, float maxHealth)
    {
        // 更新血量文本
        healthText?.text = $"血量：{health}/{maxHealth}";
        //        ^^
        //        如果healthText为null，不执行
        
        // 更新血条填充量
        if(healthBar != null)
        {
            healthBar.fillAmount = health / maxHealth;
        }
        
        // 或者使用?.运算符（但fillAmount是属性，需要先检查）
        // healthBar?.fillAmount = health / maxHealth;  // 这样也可以
        
        Debug.Log($"更新血量：{health}/{maxHealth}");
    }
    
    // 显示游戏结束面板
    public void ShowGameOver()
    {
        gameOverPanel?.SetActive(true);
        //           ^^
        //           如果gameOverPanel为null，不执行
        
        Debug.Log("显示游戏结束面板");
    }
    
    // 隐藏游戏结束面板
    public void HideGameOver()
    {
        gameOverPanel?.SetActive(false);
        Debug.Log("隐藏游戏结束面板");
    }
    
    // 额外方法：获取当前分数（安全）
    public int GetCurrentScore()
    {
        // 尝试解析文本，如果失败返回0
        if(int.TryParse(scoreText?.text?.Replace("分数：", ""), out int score))
        {
            return score;
        }
        return 0;
    }
}

// 使用示例
public class GameController : MonoBehaviour
{
    private UIManager uiManager;
    private int score = 0;
    private float health = 100f;
    private float maxHealth = 100f;
    
    void Start()
    {
        uiManager = GetComponent<UIManager>();
        
        // 初始化UI
        uiManager?.UpdateScore(score);
        uiManager?.UpdateHealth(health, maxHealth);
        uiManager?.HideGameOver();
    }
    
    void Update()
    {
        // 按空格增加分数
        if(Input.GetKeyDown(KeyCode.Space))
        {
            score += 10;
            uiManager?.UpdateScore(score);
        }
        
        // 按H减少血量
        if(Input.GetKeyDown(KeyCode.H))
        {
            health -= 10;
            uiManager?.UpdateHealth(health, maxHealth);
            
            if(health <= 0)
            {
                uiManager?.ShowGameOver();
            }
        }
    }
}
```

### 练习题2：安全的对象查找

**任务：** 创建一个工具类，安全地查找和访问游戏对象

```csharp
public class SafeObjectFinder : MonoBehaviour
{
    // TODO: 实现以下方法，使用?.和??运算符
    // 1. FindObjectByName(string name) - 查找对象，找不到返回null
    // 2. GetObjectPosition(string name) - 获取对象位置，找不到返回Vector3.zero
    // 3. GetComponentSafe<T>(GameObject obj) - 安全获取组件，找不到返回null
    // 4. IsObjectActive(string name) - 检查对象是否激活，找不到返回false
}
```

**答案：**

```csharp
public class SafeObjectFinder : MonoBehaviour
{
    // 查找对象，找不到返回null
    public GameObject FindObjectByName(string name)
    {
        GameObject obj = GameObject.Find(name);
        
        if(obj != null)
        {
            Debug.Log($"找到对象：{name}");
        }
        else
        {
            Debug.Log($"未找到对象：{name}");
        }
        
        return obj;
    }
    
    // 获取对象位置，找不到返回Vector3.zero
    public Vector3 GetObjectPosition(string name)
    {
        GameObject obj = GameObject.Find(name);
        return obj?.transform.position ?? Vector3.zero;
        //     ^^                      ^^
        //     如果obj为null，返回null
        //                             如果为null，返回Vector3.zero
    }
    
    // 安全获取组件，找不到返回null
    public T GetComponentSafe<T>(GameObject obj) where T : Component
    {
        return obj?.GetComponent<T>();
        //     ^^
        //     如果obj为null，返回null
        //     如果obj不为null，返回GetComponent<T>()的结果
    }
    
    // 检查对象是否激活，找不到返回false
    public bool IsObjectActive(string name)
    {
        GameObject obj = GameObject.Find(name);
        return obj?.activeInHierarchy ?? false;
        //     ^^                    ^^
        //     如果obj为null，返回null
        //                            如果为null，返回false
    }
    
    // 额外方法：安全设置对象位置
    public void SetObjectPosition(string name, Vector3 position)
    {
        GameObject obj = GameObject.Find(name);
        
        if(obj?.transform != null)
        {
            obj.transform.position = position;
            Debug.Log($"设置 {name} 位置为 {position}");
        }
        else
        {
            Debug.Log($"无法设置位置，对象 {name} 不存在");
        }
    }
    
    // 额外方法：安全销毁对象
    public void DestroyObjectSafe(string name)
    {
        GameObject obj = GameObject.Find(name);
        
        if(obj != null)
        {
            Destroy(obj);
            Debug.Log($"销毁对象：{name}");
        }
        else
        {
            Debug.Log($"无法销毁，对象 {name} 不存在");
        }
    }
}

// 使用示例
public class ObjectFinderExample : MonoBehaviour
{
    private SafeObjectFinder finder;
    
    void Start()
    {
        finder = gameObject.AddComponent<SafeObjectFinder>();
        
        // 查找对象
        GameObject player = finder.FindObjectByName("Player");
        
        // 获取位置
        Vector3 enemyPos = finder.GetObjectPosition("Enemy");
        Debug.Log($"敌人位置：{enemyPos}");
        
        // 安全获取组件
        Rigidbody2D rb = finder.GetComponentSafe<Rigidbody2D>(player);
        rb?.velocity = new Vector2(5, 0);
        
        // 检查对象是否激活
        bool isActive = finder.IsObjectActive("Boss");
        Debug.Log($"Boss是否激活：{isActive}");
    }
}
```

---

## 7. 属性（Property）

### 什么是属性？

属性是一种特殊的成员，它提供了灵活的机制来读取、写入或计算私有字段的值。属性看起来像字段，但实际上是方法。

**生活中的类比：**
想象你家的保险箱：
- 字段：直接把钱放在桌子上，任何人都能拿走
- 属性：钱放在保险箱里，需要通过密码（get/set方法）才能存取，你可以控制谁能存、谁能取

### 为什么需要属性？

**问题：直接使用公共字段**

```csharp
public class Player : MonoBehaviour
{
    public int health = 100;  // 任何人都能直接修改
    
    void TakeDamage(int damage)
    {
        health -= damage;
    }
}

// 其他脚本中
public class Enemy : MonoBehaviour
{
    public Player player;
    
    void Attack()
    {
        // 问题：可以直接修改，没有任何限制
        player.health = -999;  // 血量变成负数！
        player.health = 999999;  // 血量变成无限大！
    }
}
```

**解决：使用属性**

```csharp
public class Player : MonoBehaviour
{
    private int health = 100;  // 私有字段，外部无法直接访问
    
    // 属性：控制如何访问health
    public int Health
    {
        get { return health; }  // 读取时执行
        set 
        { 
            // 写入时执行，可以添加验证
            if(value < 0)
            {
                health = 0;  // 血量不能小于0
            }
            else if(value > 100)
            {
                health = 100;  // 血量不能大于100
            }
            else
            {
                health = value;
            }
            
            Debug.Log($"血量改变：{health}");
        }
    }
}

// 其他脚本中
public class Enemy : MonoBehaviour
{
    public Player player;
    
    void Attack()
    {
        player.Health = -999;  // 实际会被限制为0
        player.Health = 999999;  // 实际会被限制为100
    }
}
```

### 属性的基本语法详解

#### 完整属性（Full Property）

```csharp
public class Character : MonoBehaviour
{
    private int health;  // 私有字段（backing field）
    
    // 完整属性
    public int Health
    //         ^^^^^^
    //         属性名（通常首字母大写）
    {
        get
        //^
        //获取器：当读取属性时执行
        {
            Debug.Log("读取血量");
            return health;
        }
        set
        //^
        //设置器：当写入属性时执行
        {
            Debug.Log($"设置血量：{value}");
            //                    ^^^^^
            //                    value是关键字，代表传入的值
            health = value;
        }
    }
}
```

**详细解释：**

```csharp
// 使用属性
Character character = new Character();

// 读取属性（调用get）
int currentHealth = character.Health;
//                            ^^^^^^
//                            调用Health的get方法
//                            ↓
//                            执行：return health;

// 写入属性（调用set）
character.Health = 50;
//                 ^^
//                 这个值会传递给set方法的value参数
//                 ↓
//                 执行：health = value; （value = 50）
```

**执行流程：**

```
character.Health = 50;
↓
调用Health的set方法
↓
value = 50
↓
执行set方法体
↓
health = 50

int hp = character.Health;
↓
调用Health的get方法
↓
执行get方法体
↓
return health;
↓
hp = health的值
```

#### 自动属性（Auto Property）

如果不需要额外的逻辑，可以使用自动属性：

```csharp
public class Player : MonoBehaviour
{
    // 自动属性：编译器自动创建私有字段
    public int Score { get; set; }
    //                 ^^^  ^^^
    //                 自动生成get和set方法
    
    // 等价于：
    // private int score;
    // public int Score
    // {
    //     get { return score; }
    //     set { score = value; }
    // }
    
    // 带初始值的自动属性
    public string PlayerName { get; set; } = "未命名玩家";
    
    // 只读自动属性（只有get，没有set）
    public int MaxHealth { get; } = 100;
}
```

**详细解释：**

```csharp
public int Score { get; set; }

编译器会自动生成：
1. 一个私有字段（你看不到，但它存在）
2. get方法：返回这个私有字段
3. set方法：设置这个私有字段

使用：
player.Score = 100;  // 调用set
int score = player.Score;  // 调用get
```

#### 只读属性（Read-Only Property）

只有get，没有set的属性：

```csharp
public class Player : MonoBehaviour
{
    private int level = 1;
    
    // 只读属性：外部只能读取，不能修改
    public int Level
    {
        get { return level; }
        // 没有set方法
    }
    
    // 可以在类内部修改
    public void LevelUp()
    {
        level++;  // 内部可以修改私有字段
        Debug.Log($"升级到 {level} 级");
    }
}

// 使用
public class GameManager : MonoBehaviour
{
    public Player player;
    
    void Start()
    {
        int currentLevel = player.Level;  // 可以读取
        // player.Level = 10;  // 错误！不能修改只读属性
        
        player.LevelUp();  // 通过方法修改
    }
}
```

#### 只写属性（Write-Only Property）

只有set，没有get的属性（少见）：

```csharp
public class SecuritySystem : MonoBehaviour
{
    private string password;
    
    // 只写属性：只能设置，不能读取
    public string Password
    {
        set 
        { 
            password = value;
            Debug.Log("密码已设置（不显示内容）");
        }
        // 没有get方法
    }
    
    public bool CheckPassword(string input)
    {
        return input == password;
    }
}
```

#### 计算属性（Computed Property）

属性的值是计算出来的，不存储在字段中：

```csharp
public class Player : MonoBehaviour
{
    public int strength = 10;
    public int agility = 8;
    public int intelligence = 12;
    
    // 计算属性：总属性值
    public int TotalStats
    {
        get 
        { 
            return strength + agility + intelligence;
            // 每次读取时重新计算
        }
    }
    
    // 计算属性：战斗力
    public int CombatPower
    {
        get
        {
            return strength * 2 + agility + intelligence / 2;
        }
    }
}

// 使用
public class StatsDisplay : MonoBehaviour
{
    public Player player;
    
    void Update()
    {
        int total = player.TotalStats;  // 每次都会重新计算
        Debug.Log($"总属性：{total}");
    }
}
```

**执行流程：**

```
int total = player.TotalStats;
↓
调用TotalStats的get方法
↓
计算：strength + agility + intelligence
↓
返回计算结果
↓
total = 计算结果
```

### 属性的访问修饰符

可以为get和set设置不同的访问级别：

```csharp
public class Player : MonoBehaviour
{
    // 公共读取，私有写入
    public int Health { get; private set; } = 100;
    //                      ^^^^^^^
    //                      只有类内部可以修改
    
    // 公共读取，受保护写入
    public int Mana { get; protected set; } = 50;
    //                    ^^^^^^^^^
    //                    只有类内部和子类可以修改
    
    // 内部方法可以修改
    public void TakeDamage(int damage)
    {
        Health -= damage;  // 类内部可以修改
        if(Health < 0) Health = 0;
    }
    
    public void UseMana(int amount)
    {
        Mana -= amount;  // 类内部可以修改
        if(Mana < 0) Mana = 0;
    }
}

// 使用
public class Enemy : MonoBehaviour
{
    public Player player;
    
    void Attack()
    {
        int hp = player.Health;  // 可以读取
        // player.Health = 0;  // 错误！不能修改
        
        player.TakeDamage(10);  // 通过方法修改
    }
}
```

### Unity实际应用：角色属性系统

```csharp
public class Character : MonoBehaviour
{
    // 私有字段
    private int health;
    private int maxHealth = 100;
    private float moveSpeed;
    
    // 血量属性（带验证）
    public int Health
    {
        get { return health; }
        set
        {
            // 限制范围
            health = Mathf.Clamp(value, 0, maxHealth);
            
            // 触发事件
            OnHealthChanged?.Invoke(health, maxHealth);
            
            // 检查死亡
            if(health <= 0)
            {
                Die();
            }
            
            Debug.Log($"血量：{health}/{maxHealth}");
        }
    }
    
    // 最大血量属性
    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = Mathf.Max(value, 1);  // 至少为1
            
            // 调整当前血量
            if(health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }
    
    // 血量百分比（计算属性）
    public float HealthPercent
    {
        get { return (float)health / maxHealth; }
    }
    
    // 移动速度属性
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set
        {
            moveSpeed = Mathf.Max(value, 0);  // 不能为负数
            Debug.Log($"移动速度：{moveSpeed}");
        }
    }
    
    // 是否存活（只读属性）
    public bool IsAlive
    {
        get { return health > 0; }
    }
    
    // 事件
    public event System.Action<int, int> OnHealthChanged;
    
    void Start()
    {
        Health = maxHealth;  // 初始化血量
        MoveSpeed = 5f;
    }
    
    void Die()
    {
        Debug.Log($"{name} 死亡");
        gameObject.SetActive(false);
    }
}

// 使用示例
public class HealthBar : MonoBehaviour
{
    public Character character;
    public UnityEngine.UI.Image fillImage;
    
    void Start()
    {
        // 订阅血量改变事件
        character.OnHealthChanged += UpdateHealthBar;
    }
    
    void UpdateHealthBar(int current, int max)
    {
        // 使用HealthPercent属性
        fillImage.fillAmount = character.HealthPercent;
    }
}
```

**执行流程详解：**

```
character.Health = 50;
↓
调用Health的set方法
↓
value = 50
↓
health = Mathf.Clamp(50, 0, 100) = 50
↓
触发OnHealthChanged事件
↓
检查是否死亡（50 > 0，未死亡）
↓
输出日志："血量：50/100"

float percent = character.HealthPercent;
↓
调用HealthPercent的get方法
↓
计算：(float)50 / 100 = 0.5
↓
返回0.5
```

### Unity实际应用：物品系统

```csharp
[System.Serializable]
public class Item
{
    private string itemName;
    private int quantity;
    private int maxStack = 99;
    
    // 物品名称（只读）
    public string ItemName
    {
        get { return itemName; }
    }
    
    // 数量（带验证）
    public int Quantity
    {
        get { return quantity; }
        set
        {
            quantity = Mathf.Clamp(value, 0, maxStack);
            Debug.Log($"{itemName} 数量：{quantity}");
        }
    }
    
    // 最大堆叠数
    public int MaxStack
    {
        get { return maxStack; }
        set { maxStack = Mathf.Max(value, 1); }
    }
    
    // 是否已满（只读）
    public bool IsFull
    {
        get { return quantity >= maxStack; }
    }
    
    // 是否为空（只读）
    public bool IsEmpty
    {
        get { return quantity <= 0; }
    }
    
    // 构造函数
    public Item(string name, int quantity = 1)
    {
        this.itemName = name;
        this.Quantity = quantity;  // 使用属性，触发验证
    }
    
    // 添加物品
    public int Add(int amount)
    {
        int oldQuantity = quantity;
        Quantity += amount;  // 使用属性
        return quantity - oldQuantity;  // 返回实际添加的数量
    }
    
    // 移除物品
    public int Remove(int amount)
    {
        int oldQuantity = quantity;
        Quantity -= amount;  // 使用属性
        return oldQuantity - quantity;  // 返回实际移除的数量
    }
}

// 使用示例
public class Inventory : MonoBehaviour
{
    private List<Item> items = new List<Item>();
    
    void Start()
    {
        // 创建物品
        Item potion = new Item("生命药水", 5);
        items.Add(potion);
        
        // 读取属性
        Debug.Log($"物品：{potion.ItemName}");
        Debug.Log($"数量：{potion.Quantity}");
        Debug.Log($"是否已满：{potion.IsFull}");
        
        // 添加物品
        int added = potion.Add(10);
        Debug.Log($"添加了 {added} 个");
        
        // 尝试添加超过上限
        potion.Add(100);  // 会被限制在99
        Debug.Log($"当前数量：{potion.Quantity}");
    }
}
```

### Unity实际应用：单例模式

```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    // 单例属性
    public static GameManager Instance
    {
        get
        {
            // 如果实例不存在，查找或创建
            if(instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                
                if(instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    
    // 游戏状态
    private bool isPaused = false;
    
    public bool IsPaused
    {
        get { return isPaused; }
        set
        {
            isPaused = value;
            Time.timeScale = isPaused ? 0 : 1;  // 暂停或恢复游戏
            Debug.Log(isPaused ? "游戏暂停" : "游戏继续");
        }
    }
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

// 使用示例
public class PauseMenu : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // 切换暂停状态
            GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
        }
    }
}
```

### 练习题1：经验值系统

**任务：** 创建一个经验值系统，当经验值达到上限时自动升级

```csharp
public class Player : MonoBehaviour
{
    // TODO: 实现以下属性
    // 1. Level（等级）- 只读，初始为1
    // 2. Experience（经验值）- 可读写，当经验值>=ExpToNextLevel时自动升级
    // 3. ExpToNextLevel（升级所需经验）- 只读，计算公式：level * 100
    // 4. ExpPercent（经验值百分比）- 只读，用于显示经验条
    
    // 提示：
    // - 升级时，经验值应该保留超出的部分
    // - 例如：需要100经验升级，当前有120经验，升级后应该剩余20经验
}
```

**答案：**

```csharp
public class Player : MonoBehaviour
{
    private int level = 1;
    private int experience = 0;
    
    // 等级（只读）
    public int Level
    {
        get { return level; }
    }
    
    // 经验值（带自动升级）
    public int Experience
    {
        get { return experience; }
        set
        {
            experience = Mathf.Max(value, 0);  // 不能为负数
            
            // 检查是否可以升级
            while(experience >= ExpToNextLevel)
            {
                LevelUp();
            }
            
            Debug.Log($"经验值：{experience}/{ExpToNextLevel}");
        }
    }
    
    // 升级所需经验（只读，计算属性）
    public int ExpToNextLevel
    {
        get { return level * 100; }
    }
    
    // 经验值百分比（只读，计算属性）
    public float ExpPercent
    {
        get { return (float)experience / ExpToNextLevel; }
    }
    
    // 升级方法
    private void LevelUp()
    {
        experience -= ExpToNextLevel;  // 扣除升级所需经验
        level++;
        Debug.Log($"升级！当前等级：{level}");
    }
    
    // 测试方法
    void Start()
    {
        Debug.Log($"初始等级：{Level}");
        Debug.Log($"升级所需经验：{ExpToNextLevel}");
        
        // 添加经验
        Experience = 50;
        Debug.Log($"经验百分比：{ExpPercent * 100}%");
        
        // 添加足够升级的经验
        Experience += 60;  // 总共110，应该升到2级，剩余10经验
        Debug.Log($"当前等级：{Level}");
        Debug.Log($"剩余经验：{Experience}");
        
        // 添加大量经验，连续升级
        Experience += 500;  // 应该连续升级多次
        Debug.Log($"最终等级：{Level}");
        Debug.Log($"最终经验：{Experience}");
    }
}
```

**执行流程：**

```
Experience = 50;
↓
调用Experience的set方法
↓
experience = 50
↓
检查：50 >= 100？否
↓
不升级

Experience += 60;  // 现在是110
↓
调用Experience的set方法
↓
experience = 110
↓
检查：110 >= 100？是
↓
调用LevelUp()
↓
experience = 110 - 100 = 10
↓
level = 2
↓
检查：10 >= 200？否
↓
升级完成
```

### 练习题2：温度转换器

**任务：** 创建一个温度类，支持摄氏度和华氏度的自动转换

```csharp
public class Temperature : MonoBehaviour
{
    // TODO: 实现以下属性
    // 1. Celsius（摄氏度）- 可读写
    // 2. Fahrenheit（华氏度）- 可读写，自动转换
    // 3. Kelvin（开尔文）- 只读，自动计算
    
    // 转换公式：
    // 华氏度 = 摄氏度 * 9/5 + 32
    // 摄氏度 = (华氏度 - 32) * 5/9
    // 开尔文 = 摄氏度 + 273.15
}
```

**答案：**

```csharp
public class Temperature : MonoBehaviour
{
    private float celsius = 0;  // 内部存储摄氏度
    
    // 摄氏度
    public float Celsius
    {
        get { return celsius; }
        set 
        { 
            celsius = value;
            Debug.Log($"温度设置为：{celsius}°C");
        }
    }
    
    // 华氏度（自动转换）
    public float Fahrenheit
    {
        get 
        { 
            return celsius * 9f / 5f + 32f;
        }
        set
        {
            celsius = (value - 32f) * 5f / 9f;
            Debug.Log($"温度设置为：{value}°F（{celsius}°C）");
        }
    }
    
    // 开尔文（只读，自动计算）
    public float Kelvin
    {
        get { return celsius + 273.15f; }
    }
    
    // 温度描述（只读，计算属性）
    public string Description
    {
        get
        {
            if(celsius < 0) return "冰冻";
            if(celsius < 10) return "寒冷";
            if(celsius < 20) return "凉爽";
            if(celsius < 30) return "温暖";
            return "炎热";
        }
    }
    
    // 测试方法
    void Start()
    {
        // 设置摄氏度
        Celsius = 25;
        Debug.Log($"摄氏度：{Celsius}°C");
        Debug.Log($"华氏度：{Fahrenheit}°F");
        Debug.Log($"开尔文：{Kelvin}K");
        Debug.Log($"描述：{Description}");
        
        Debug.Log("---");
        
        // 设置华氏度
        Fahrenheit = 100;
        Debug.Log($"摄氏度：{Celsius}°C");
        Debug.Log($"华氏度：{Fahrenheit}°F");
        Debug.Log($"开尔文：{Kelvin}K");
        Debug.Log($"描述：{Description}");
    }
}
```

**执行流程：**

```
Celsius = 25;
↓
调用Celsius的set方法
↓
celsius = 25

float f = Fahrenheit;
↓
调用Fahrenheit的get方法
↓
计算：25 * 9/5 + 32 = 77
↓
返回77

Fahrenheit = 100;
↓
调用Fahrenheit的set方法
↓
计算：(100 - 32) * 5/9 = 37.78
↓
celsius = 37.78

float k = Kelvin;
↓
调用Kelvin的get方法
↓
计算：37.78 + 273.15 = 310.93
↓
返回310.93
```

---

## 8. 接口（Interface）

### 什么是接口？

接口是一种契约，定义了类必须实现的成员（方法、属性、事件等），但不提供实现。接口使用`interface`关键字定义。

**生活中的类比：**
想象你要招聘员工：
- 接口：职位描述（必须会打字、会说英语、会用电脑）
- 实现接口：应聘者必须具备这些技能，但每个人的实现方式不同
- 多个接口：一个人可以同时是"程序员"、"设计师"、"翻译"

### 为什么需要接口？

**问题：没有接口时**

```csharp
public class Player : MonoBehaviour
{
    public void TakeDamage(int damage) { }
}

public class Enemy : MonoBehaviour
{
    public void TakeDamage(int damage) { }
}

public class Barrel : MonoBehaviour
{
    public void TakeDamage(int damage) { }
}

// 问题：如何统一处理这些不同的类？
public class Weapon : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // 需要分别检查每种类型
        Player player = other.GetComponent<Player>();
        if(player != null)
        {
            player.TakeDamage(10);
            return;
        }
        
        Enemy enemy = other.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(10);
            return;
        }
        
        Barrel barrel = other.GetComponent<Barrel>();
        if(barrel != null)
        {
            barrel.TakeDamage(10);
        }
    }
}
```

**解决：使用接口**

```csharp
// 定义接口
public interface IDamageable
{
    void TakeDamage(int damage);
}

// 实现接口
public class Player : MonoBehaviour, IDamageable
{
    public void TakeDamage(int damage)
    {
        Debug.Log($"玩家受到 {damage} 伤害");
    }
}

public class Enemy : MonoBehaviour, IDamageable
{
    public void TakeDamage(int damage)
    {
        Debug.Log($"敌人受到 {damage} 伤害");
    }
}

public class Barrel : MonoBehaviour, IDamageable
{
    public void TakeDamage(int damage)
    {
        Debug.Log($"木桶受到 {damage} 伤害");
    }
}

// 统一处理
public class Weapon : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // 只需要检查一次
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeDamage(10);
        }
    }
}
```


### 接口的基本语法详解

#### 定义接口

```csharp
public interface IMovable
//              ^
//              接口名通常以I开头
{
    // 方法声明（没有实现）
    void Move(Vector2 direction);
    //                          ^
    //                          没有方法体
    
    // 属性声明
    float Speed { get; set; }
    
    // 只读属性
    bool IsMoving { get; }
}
```

**详细解释：**

```csharp
接口的特点：
1. 使用interface关键字
2. 接口名通常以I开头（约定俗成）
3. 只声明成员，不提供实现
4. 接口成员默认是public的
5. 不能包含字段（变量）
```

#### 实现接口

```csharp
public class Player : MonoBehaviour, IMovable
//                                   ^
//                                   实现IMovable接口
{
    private float speed = 5f;
    
    // 实现接口的方法
    public void Move(Vector2 direction)
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    
    // 实现接口的属性
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    
    // 实现接口的只读属性
    public bool IsMoving
    {
        get { return speed > 0; }
    }
}
```

**执行流程：**

```
Player player = new Player();
IMovable movable = player;  // 可以转换为接口类型
movable.Move(Vector2.right);
↓
调用Player的Move方法
↓
执行：transform.Translate(...)
```


#### 实现多个接口

C#不支持多重继承，但可以实现多个接口：

```csharp
public interface IDamageable
{
    void TakeDamage(int damage);
    int Health { get; }
}

public interface IHealable
{
    void Heal(int amount);
}

public interface IMovable
{
    void Move(Vector2 direction);
}

// 同时实现多个接口
public class Player : MonoBehaviour, IDamageable, IHealable, IMovable
//                                   ^           ^          ^
//                                   用逗号分隔多个接口
{
    private int health = 100;
    private float speed = 5f;
    
    // 实现IDamageable
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"受到伤害，剩余血量：{health}");
    }
    
    public int Health
    {
        get { return health; }
    }
    
    // 实现IHealable
    public void Heal(int amount)
    {
        health += amount;
        Debug.Log($"恢复血量，当前血量：{health}");
    }
    
    // 实现IMovable
    public void Move(Vector2 direction)
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
```


### Unity实际应用：伤害系统

```csharp
// 定义可受伤接口
public interface IDamageable
{
    void TakeDamage(int damage);
    void Die();
    int Health { get; }
    bool IsAlive { get; }
}

// 玩家实现
public class Player : MonoBehaviour, IDamageable
{
    private int health = 100;
    
    public void TakeDamage(int damage)
    {
        if(!IsAlive) return;
        
        health -= damage;
        Debug.Log($"玩家受到 {damage} 伤害，剩余 {health}");
        
        if(health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        Debug.Log("玩家死亡");
        gameObject.SetActive(false);
    }
    
    public int Health { get { return health; } }
    public bool IsAlive { get { return health > 0; } }
}

// 敌人实现
public class Enemy : MonoBehaviour, IDamageable
{
    private int health = 50;
    
    public void TakeDamage(int damage)
    {
        if(!IsAlive) return;
        
        health -= damage;
        Debug.Log($"敌人受到 {damage} 伤害，剩余 {health}");
        
        if(health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        Debug.Log("敌人死亡");
        Destroy(gameObject);
    }
    
    public int Health { get { return health; } }
    public bool IsAlive { get { return health > 0; } }
}

// 武器系统
public class Weapon : MonoBehaviour
{
    public int damage = 10;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // 统一处理所有可受伤对象
        IDamageable target = other.GetComponent<IDamageable>();
        if(target != null && target.IsAlive)
        {
            target.TakeDamage(damage);
        }
    }
}
```

**执行流程：**

```
武器碰到玩家
↓
GetComponent<IDamageable>()
↓
找到Player组件（实现了IDamageable）
↓
target.TakeDamage(10)
↓
调用Player的TakeDamage方法
↓
health -= 10
↓
检查是否死亡
```


### Unity实际应用：交互系统

```csharp
// 定义可交互接口
public interface IInteractable
{
    void Interact();
    string GetInteractPrompt();
    bool CanInteract { get; }
}

// 门实现
public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    
    public void Interact()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "门打开了" : "门关闭了");
    }
    
    public string GetInteractPrompt()
    {
        return isOpen ? "按E关门" : "按E开门";
    }
    
    public bool CanInteract { get { return true; } }
}

// 宝箱实现
public class Chest : MonoBehaviour, IInteractable
{
    private bool isOpened = false;
    
    public void Interact()
    {
        if(isOpened) return;
        
        isOpened = true;
        Debug.Log("打开宝箱，获得物品！");
    }
    
    public string GetInteractPrompt()
    {
        return isOpened ? "已打开" : "按E打开宝箱";
    }
    
    public bool CanInteract { get { return !isOpened; } }
}

// NPC实现
public class NPC : MonoBehaviour, IInteractable
{
    public string npcName = "村民";
    public string dialogue = "你好，旅行者！";
    
    public void Interact()
    {
        Debug.Log($"{npcName}：{dialogue}");
    }
    
    public string GetInteractPrompt()
    {
        return $"按E与{npcName}对话";
    }
    
    public bool CanInteract { get { return true; } }
}

// 玩家交互控制器
public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    
    void Update()
    {
        // 检测附近的可交互对象
        DetectInteractable();
        
        // 按E键交互
        if(Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            if(currentInteractable.CanInteract)
            {
                currentInteractable.Interact();
            }
        }
    }
    
    void DetectInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
        
        currentInteractable = null;
        
        foreach(Collider2D col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if(interactable != null && interactable.CanInteract)
            {
                currentInteractable = interactable;
                Debug.Log(interactable.GetInteractPrompt());
                break;
            }
        }
    }
}
```


### Unity实际应用：存档系统

```csharp
// 定义可保存接口
public interface ISaveable
{
    string GetSaveData();
    void LoadData(string data);
}

// 玩家数据
[System.Serializable]
public class PlayerData
{
    public int health;
    public int level;
    public Vector3 position;
}

// 玩家实现存档
public class Player : MonoBehaviour, ISaveable
{
    public int health = 100;
    public int level = 1;
    
    public string GetSaveData()
    {
        PlayerData data = new PlayerData
        {
            health = this.health,
            level = this.level,
            position = transform.position
        };
        
        return JsonUtility.ToJson(data);
    }
    
    public void LoadData(string jsonData)
    {
        PlayerData data = JsonUtility.FromJson<PlayerData>(jsonData);
        
        health = data.health;
        level = data.level;
        transform.position = data.position;
        
        Debug.Log($"加载玩家数据：等级{level}，血量{health}");
    }
}

// 存档管理器
public class SaveManager : MonoBehaviour
{
    public void SaveAll()
    {
        // 查找所有实现ISaveable的对象
        ISaveable[] saveables = FindObjectsOfType<MonoBehaviour>()
            .OfType<ISaveable>()
            .ToArray();
        
        foreach(ISaveable saveable in saveables)
        {
            string data = saveable.GetSaveData();
            // 保存到文件或PlayerPrefs
            Debug.Log($"保存数据：{data}");
        }
    }
    
    public void LoadAll()
    {
        ISaveable[] saveables = FindObjectsOfType<MonoBehaviour>()
            .OfType<ISaveable>()
            .ToArray();
        
        foreach(ISaveable saveable in saveables)
        {
            // 从文件或PlayerPrefs加载
            string data = "";  // 实际应该从存档读取
            saveable.LoadData(data);
        }
    }
}
```


### 接口继承

接口可以继承其他接口：

```csharp
// 基础接口
public interface IEntity
{
    string Name { get; }
    Vector3 Position { get; }
}

// 继承IEntity
public interface ILivingEntity : IEntity
{
    int Health { get; }
    void TakeDamage(int damage);
}

// 继承ILivingEntity
public interface IPlayer : ILivingEntity
{
    int Level { get; }
    void LevelUp();
}

// 实现IPlayer（必须实现所有继承的接口成员）
public class Player : MonoBehaviour, IPlayer
{
    // IEntity成员
    public string Name { get; set; } = "玩家";
    public Vector3 Position { get { return transform.position; } }
    
    // ILivingEntity成员
    private int health = 100;
    public int Health { get { return health; } }
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
    
    // IPlayer成员
    private int level = 1;
    public int Level { get { return level; } }
    public void LevelUp()
    {
        level++;
    }
}
```


### 练习题1：商店系统

**任务：** 创建一个商店系统，支持购买和出售物品

```csharp
// TODO: 定义以下接口
// 1. IItem - 物品接口
//    - string Name { get; }
//    - int Price { get; }
//    - string Description { get; }
//
// 2. IBuyable - 可购买接口
//    - bool CanBuy(int money);
//    - void Buy();
//
// 3. ISellable - 可出售接口
//    - int GetSellPrice();
//    - void Sell();

// TODO: 创建以下类实现接口
// 1. Weapon - 武器（实现IItem和IBuyable）
// 2. Potion - 药水（实现IItem、IBuyable和ISellable）
```

**答案：**

```csharp
// 物品接口
public interface IItem
{
    string Name { get; }
    int Price { get; }
    string Description { get; }
}

// 可购买接口
public interface IBuyable
{
    bool CanBuy(int money);
    void Buy();
}

// 可出售接口
public interface ISellable
{
    int GetSellPrice();
    void Sell();
}

// 武器类
public class Weapon : IItem, IBuyable
{
    public string Name { get; private set; }
    public int Price { get; private set; }
    public string Description { get; private set; }
    public int Damage { get; private set; }
    
    public Weapon(string name, int price, int damage)
    {
        Name = name;
        Price = price;
        Damage = damage;
        Description = $"伤害：{damage}";
    }
    
    public bool CanBuy(int money)
    {
        return money >= Price;
    }
    
    public void Buy()
    {
        Debug.Log($"购买了 {Name}，花费 {Price} 金币");
    }
}

// 药水类
public class Potion : IItem, IBuyable, ISellable
{
    public string Name { get; private set; }
    public int Price { get; private set; }
    public string Description { get; private set; }
    public int HealAmount { get; private set; }
    
    public Potion(string name, int price, int healAmount)
    {
        Name = name;
        Price = price;
        HealAmount = healAmount;
        Description = $"恢复 {healAmount} 血量";
    }
    
    public bool CanBuy(int money)
    {
        return money >= Price;
    }
    
    public void Buy()
    {
        Debug.Log($"购买了 {Name}，花费 {Price} 金币");
    }
    
    public int GetSellPrice()
    {
        return Price / 2;  // 出售价格是购买价格的一半
    }
    
    public void Sell()
    {
        Debug.Log($"出售了 {Name}，获得 {GetSellPrice()} 金币");
    }
}

// 商店系统
public class Shop : MonoBehaviour
{
    private int playerMoney = 1000;
    
    void Start()
    {
        // 创建物品
        Weapon sword = new Weapon("铁剑", 100, 10);
        Potion healthPotion = new Potion("生命药水", 50, 20);
        
        Debug.Log($"玩家金币：{playerMoney}");
        
        // 购买武器
        BuyItem(sword);
        
        // 购买药水
        BuyItem(healthPotion);
        
        // 出售药水
        SellItem(healthPotion);
    }
    
    void BuyItem(IBuyable item)
    {
        IItem itemInfo = item as IItem;
        
        if(item.CanBuy(playerMoney))
        {
            playerMoney -= itemInfo.Price;
            item.Buy();
            Debug.Log($"剩余金币：{playerMoney}");
        }
        else
        {
            Debug.Log($"金币不足，无法购买 {itemInfo.Name}");
        }
    }
    
    void SellItem(ISellable item)
    {
        playerMoney += item.GetSellPrice();
        item.Sell();
        Debug.Log($"剩余金币：{playerMoney}");
    }
}
```


### 练习题2：技能系统

**任务：** 创建一个技能系统，支持不同类型的技能

```csharp
// TODO: 定义以下接口
// 1. ISkill - 技能接口
//    - string Name { get; }
//    - int ManaCost { get; }
//    - float Cooldown { get; }
//    - void Use();
//    - bool CanUse();
//
// 2. IDamageSkill - 伤害技能接口（继承ISkill）
//    - int Damage { get; }
//
// 3. IHealSkill - 治疗技能接口（继承ISkill）
//    - int HealAmount { get; }

// TODO: 创建以下类
// 1. Fireball - 火球术（实现IDamageSkill）
// 2. Heal - 治疗术（实现IHealSkill）
```

**答案：**

```csharp
// 技能接口
public interface ISkill
{
    string Name { get; }
    int ManaCost { get; }
    float Cooldown { get; }
    void Use();
    bool CanUse();
}

// 伤害技能接口
public interface IDamageSkill : ISkill
{
    int Damage { get; }
}

// 治疗技能接口
public interface IHealSkill : ISkill
{
    int HealAmount { get; }
}

// 火球术
public class Fireball : IDamageSkill
{
    public string Name { get { return "火球术"; } }
    public int ManaCost { get { return 20; } }
    public float Cooldown { get { return 2f; } }
    public int Damage { get { return 30; } }
    
    private float lastUseTime = -999f;
    
    public bool CanUse()
    {
        return Time.time - lastUseTime >= Cooldown;
    }
    
    public void Use()
    {
        if(!CanUse())
        {
            Debug.Log($"{Name} 冷却中...");
            return;
        }
        
        lastUseTime = Time.time;
        Debug.Log($"使用 {Name}！造成 {Damage} 伤害，消耗 {ManaCost} 魔法");
    }
}

// 治疗术
public class Heal : IHealSkill
{
    public string Name { get { return "治疗术"; } }
    public int ManaCost { get { return 15; } }
    public float Cooldown { get { return 3f; } }
    public int HealAmount { get { return 50; } }
    
    private float lastUseTime = -999f;
    
    public bool CanUse()
    {
        return Time.time - lastUseTime >= Cooldown;
    }
    
    public void Use()
    {
        if(!CanUse())
        {
            Debug.Log($"{Name} 冷却中...");
            return;
        }
        
        lastUseTime = Time.time;
        Debug.Log($"使用 {Name}！恢复 {HealAmount} 血量，消耗 {ManaCost} 魔法");
    }
}

// 技能管理器
public class SkillManager : MonoBehaviour
{
    private List<ISkill> skills = new List<ISkill>();
    private int currentMana = 100;
    
    void Start()
    {
        // 添加技能
        skills.Add(new Fireball());
        skills.Add(new Heal());
        
        Debug.Log("技能列表：");
        foreach(ISkill skill in skills)
        {
            Debug.Log($"- {skill.Name}（魔法消耗：{skill.ManaCost}，冷却：{skill.Cooldown}秒）");
        }
    }
    
    void Update()
    {
        // 按1使用第一个技能
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill(0);
        }
        
        // 按2使用第二个技能
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSkill(1);
        }
    }
    
    void UseSkill(int index)
    {
        if(index < 0 || index >= skills.Count) return;
        
        ISkill skill = skills[index];
        
        if(!skill.CanUse())
        {
            Debug.Log($"{skill.Name} 冷却中");
            return;
        }
        
        if(currentMana < skill.ManaCost)
        {
            Debug.Log($"魔法不足，无法使用 {skill.Name}");
            return;
        }
        
        currentMana -= skill.ManaCost;
        skill.Use();
        Debug.Log($"剩余魔法：{currentMana}");
    }
}
```

**执行流程：**

```
按下键盘1
↓
UseSkill(0)
↓
获取skills[0]（Fireball）
↓
检查CanUse()
↓
检查魔法是否足够
↓
currentMana -= 20
↓
调用skill.Use()
↓
执行Fireball的Use方法
↓
输出："使用 火球术！造成 30 伤害，消耗 20 魔法"
```

---

## 总结

恭喜你完成了C#核心概念的学习！让我们回顾一下这8个重要概念：

1. **委托（Delegate）**：方法的引用，可以像变量一样传递方法
2. **事件（Event）**：基于委托的发布-订阅模式，用于对象间通信
3. **Lambda表达式**：简洁的匿名方法写法
4. **协程（Coroutine）**：Unity特有的异步执行机制
5. **泛型（Generic）**：编写可处理多种类型的代码
6. **空值条件运算符（?.）**：安全访问可能为null的对象
7. **属性（Property）**：控制字段访问的灵活机制
8. **接口（Interface）**：定义类必须实现的契约

这些概念在Unity开发中无处不在，掌握它们将大大提升你的编程能力！

**学习建议：**
- 多练习每个概念的练习题
- 在实际项目中应用这些概念
- 尝试组合使用多个概念（例如：泛型+接口，事件+委托）
- 阅读Unity官方文档和优秀的开源项目

继续加油，祝你在Unity开发的道路上越走越远！
