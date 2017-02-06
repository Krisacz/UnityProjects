using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Controllers
{
    public class AsteroidController
    {
        //Scale from 0.1f - 5f
        private List<int> _big = new List<int>() { 0, 1, 2, 3, 4, 31 };

        //Scale from 0.1f - 5f
        private List<int> _medium = new List<int>() { 5, 10, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 32, 33, 34 };

        //Scale from 0.25f - 2f
        private List<int> _small = new List<int>() { 6, 7, 8, 9, 25, 26, 27, 28, 29, 30, 35, 36 };
        
        //asteroids_[number] e.g. asteroids_29

    }
}
