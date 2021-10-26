using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 패킷을 밀어내고 뽑아내는 역할
// 메인 스레드랑 백그라운드 스레드 네트워크를 처리하는 애들끼리 소통을 이 클래스를 이용해서
// 패킷을 밀어 두고 실제 메인스레드에선 팝으로 꺼내씀
public class Queue
{
    // 싱글턴
    public static Queue Instance { get; } = new Queue();

    Queue<Dictionary<int, object>> _queue = new Queue<Dictionary<int, object>>();
    object _lock = new object();


    // 패킷 넣어둬
    public void Push(Dictionary<int, object> result)
    {
        lock (_lock)
        {
            _queue.Enqueue(result);
        }
    }

    // 패킷 꺼내옴
    // 패킷 quque에 들어있는걸 꺼낸 다음에 패킷 번호에 따라 어떤 함수를 호출
    public void Pop()
    {
        lock (_lock)
        {
            _queue.Dequeue();
        }
    }

    public List<Dictionary<int, object>> PopAll()
    {
        List<Dictionary<int, object>> list = new List<Dictionary<int, object>>();

        lock (_lock)
        {
            while (_queue.Count > 0)
                list.Add(_queue.Dequeue());
        }

        return list;
    }
}
