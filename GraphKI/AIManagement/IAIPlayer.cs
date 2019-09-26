using GraphKI.GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.AIManagement
{
    public interface IAIPlayer
    {
        PlayerCode Player { get; }
        GameManager GameManager { get; }
    }
}
