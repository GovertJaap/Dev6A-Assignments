using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntryPoint
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var fullscreen = false;
            read_input:
            switch (Microsoft.VisualBasic.Interaction.InputBox("Which assignment shall run next? (1, 2, 3, 4, or q for quit)", "Choose assignment", VirtualCity.GetInitialValue()))
            {
                case "1":
                    using (var game = VirtualCity.RunAssignment1(SortSpecialBuildingsByDistance, fullscreen))
                        game.Run();
                    break;
                case "2":
                    using (var game = VirtualCity.RunAssignment2(FindSpecialBuildingsWithinDistanceFromHouse, fullscreen))
                        game.Run();
                    break;
                case "3":
                    using (var game = VirtualCity.RunAssignment3(FindRoute, fullscreen))
                        game.Run();
                    break;
                case "4":
                    using (var game = VirtualCity.RunAssignment4(FindRoutesToAll, fullscreen))
                        game.Run();
                    break;
                case "q":
                    return;
            }
            goto read_input;
        }


        private static IEnumerable<Vector2> SortSpecialBuildingsByDistance(Vector2 house, IEnumerable<Vector2> specialBuildings)
        {

            //specialBuildings.OrderBy(v => Vector2.Distance(v, house));
            Vector2[] array = specialBuildings.ToArray();
            Vector2 localhouse = house;
            SortMerge(localhouse, array, 0, array.Length - 1);
            return array;
        }


        static public void MainMerge(Vector2 house, Vector2[] array, int left, int mid, int right)
        {
            Vector2[] tempLeft = new Vector2[mid - left];
            Vector2[] tempRight = new Vector2[right - (mid - 1)];

            for (int j = 0; j <tempLeft.Length; j++)
            {
                tempLeft[j] = array[(left + j)];
            }
            for (int k = 0; k < tempRight.Length; k++)
            {
                tempRight[k] = array[(mid + k)];
            }
            //tempLeft = array[left, mid];
            //tempRight = array[mid+1, right];

            var l_i = 0;
            var r_i = 0;
            var i = left;

            while (i < right && l_i < tempLeft.Length && r_i < tempRight.Length)
            {
                if (Vector2.Distance(tempLeft[l_i], house) < Vector2.Distance(tempRight[r_i], house))
                {
                    array[i++] = tempLeft[l_i++];
                }
                else
                {
                    array[i++] = tempRight[r_i++];
                }
            }

            while (l_i < tempLeft.Length)
            {
                array[i++] = tempLeft[l_i++];
            }

            while (r_i < tempRight.Length)
            {
                array[i++] = tempRight[r_i++];
            }
            /*
            int[] temp = new int[50];
            int i, left_end, intArray_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            intArray_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (intArray[left] <= intArray[mid])
                    temp[tmp_pos++] = intArray[left++];
                else
                    temp[tmp_pos++] = intArray[mid++];
            }

            while (left <= left_end)
                temp[tmp_pos++] = intArray[left++];

            while (mid <= right)
                temp[tmp_pos++] = intArray[mid++];

            for (i = 0; i < intArray_elements; i++)
            {
                intArray[right] = temp[right];
                right--;
            }
            */
        }


        static public IEnumerable<Vector2> SortMerge(Vector2 house, Vector2[] array, int leftBoundArray, int rightBoundArray)
        {
            int middle;

            if (rightBoundArray > leftBoundArray)
            {
                middle = (rightBoundArray + leftBoundArray) / 2;
                SortMerge(house, array, leftBoundArray, middle);
                SortMerge(house, array, (middle + 1), rightBoundArray);

                MainMerge(house, array, leftBoundArray, (middle + 1), rightBoundArray);

            }
            return array;
        }

        interface Tree<T>
        {
            Boolean isEmpty();

            Tree<T> getLeftTree();

            Tree<T> getRightTree();

            Boolean sortedOnX();

            T getVector();
        }

        //since node and emptynode both inherit from the abstract interface Tree, they can both be used
        class Node<T> : Tree<T>
        {
            T Vector;
            Tree<T> left;
            Tree<T> right;
            Boolean sortage;

            public Boolean sortedOnX()
            {
                return sortage;
            }

            public Boolean isEmpty()
            {
                return false;
            }

            //getters
            public T getVector()
            {
                return Vector;
            }

            public Tree<T> getLeftTree()
            {
                return left;
            }

            public Tree<T> getRightTree()
            {
                return right;
            }


            //constructor
            public Node(T V, Tree<T> l, Tree<T> r, Boolean s)
            {
                Vector = V;
                left = l;
                right = r;
                sortage = s;
            }

        }

        //since node and emptynode both inherit from the abstract interface Tree, they can both be used
        class EmptyNode<T> : Tree<T>
        {
            public Boolean sortedOnX()
            {
                return false;
            }

            public Boolean isEmpty()
            {
                return true;
            }

            //getters
            public T getVector()
            {
                throw new NotImplementedException();
            }


            public Tree<T> getLeftTree()
            {
                throw new NotImplementedException();
            }

            public Tree<T> getRightTree()
            {
                throw new NotImplementedException();
            }
            //(explicit) constructor for an emptynode is not specified in the interface contract and thusly not necessary
            //same goes for setters
        }


        //Call this with nextLevelSortedOnX =true!
        static Tree<Vector2> insertIntoKD(Vector2 Vector, Tree<Vector2> root, bool isParentX)
        {
            if (root.isEmpty())
            {
                if (isParentX)
                    return new Node<Vector2>(Vector, new EmptyNode<Vector2>(), new EmptyNode<Vector2>(), false);
                else
                    return new Node<Vector2>(Vector, new EmptyNode<Vector2>(), new EmptyNode<Vector2>(), true);
            }
            if (root.sortedOnX())
            {
                if (root.getVector() == Vector)
                    return root;

                if (Vector.X < root.getVector().X)
                    return new Node<Vector2>(root.getVector(), insertIntoKD(Vector, root.getLeftTree(), root.sortedOnX()), root.getRightTree(), true);
                else
                    return new Node<Vector2>(root.getVector(), root.getLeftTree(), insertIntoKD(Vector, root.getRightTree(), root.sortedOnX()), true);
            }
            else
            {
                if (root.getVector() == Vector)
                    return root;

                if (Vector.Y < root.getVector().Y)
                    return new Node<Vector2>(root.getVector(), insertIntoKD(Vector, root.getLeftTree(), root.sortedOnX()), root.getRightTree(), false);
                else
                    return new Node<Vector2>(root.getVector(), root.getLeftTree(), insertIntoKD(Vector, root.getRightTree(), root.sortedOnX()), false);
            }
        }
        
        //Rangesearch
        static void rangeSearch(Tree<Vector2> root, Vector2 houseVector, float radius, List<Vector2> returnList)
        {
            if (root.isEmpty() == false)
            {
                if (root.sortedOnX() == true)
                {
                    //If we are WITHIN radius
                    if ((houseVector.X - root.getVector().X) < radius)
                    {
                        //Euclidean check for good measure (haha)
                        if (Vector2.Distance(root.getVector(), houseVector) <= radius)
                            returnList.Add(root.getVector());

                        //Be thorough and searche the rest too
                        rangeSearch(root.getLeftTree(), houseVector, radius, returnList);
                        rangeSearch(root.getRightTree(), houseVector, radius, returnList);
                    }
                    else if (root.getVector().X >= (houseVector.X + radius))
                    {
                        Console.WriteLine(root.getVector().X + " is bigger than " + (houseVector.X + radius) + " so we go left");
                        rangeSearch(root.getLeftTree(), houseVector, radius, returnList);
                    }
                    else if (root.getVector().X <= (houseVector.X - radius))
                    {
                        Console.WriteLine(root.getVector().X + " is smaller than " + (houseVector.X + radius) + " so we go right");
                        rangeSearch(root.getRightTree(), houseVector, radius, returnList);
                    }
                    else
                    {
                        Console.WriteLine("Not a single matching node found");
                    }
                }
                else
                {
                    //Perfect Node within range, we can return the subtree of root
                    //If we are WITHIN radius
                    if ((houseVector.Y - root.getVector().Y) <= radius)
                    {
                        //Euclidean check for good measure (haha)
                        if (Vector2.Distance(root.getVector(), houseVector) <= radius)
                            returnList.Add(root.getVector());

                        //Be thorough and searche the rest too
                        rangeSearch(root.getLeftTree(), houseVector, radius, returnList);
                        rangeSearch(root.getRightTree(), houseVector, radius, returnList);
                    }
                    else if (root.getVector().Y > (houseVector.Y + radius))
                    {
                        Console.WriteLine(root.getVector().Y + " is bigger than " + (houseVector.Y + radius) + " so we go left");
                        rangeSearch(root.getLeftTree(), houseVector, radius, returnList);
                    }
                    else if (root.getVector().Y < (houseVector.Y - radius))
                    {
                        Console.WriteLine(root.getVector().Y + " is smaller than " + (houseVector.Y + radius) + " so we go right");
                        rangeSearch(root.getRightTree(), houseVector, radius, returnList);
                    }
                    else
                    {
                        Console.WriteLine("Not a single matching node found");
                    }
                }
            }
            else
            {
                Console.WriteLine("Empty tree");
            }
        }

        private static IEnumerable<IEnumerable<Vector2>> FindSpecialBuildingsWithinDistanceFromHouse(
          IEnumerable<Vector2> specialBuildings,
          IEnumerable<Tuple<Vector2, float>> housesAndDistances)
        {
            var Tree = new EmptyNode<Vector2>() as Tree<Vector2>;
            List<Vector2> listOfBuildings = specialBuildings.ToList();

            foreach (Vector2 v in listOfBuildings)
            {
                Tree = insertIntoKD(v, Tree, Tree.sortedOnX());
            }

            List<Tuple<Vector2, float>> housesAndDistancesList = housesAndDistances.ToList();
            List<List<Vector2>> returnList = new List<List<Vector2>>();

            foreach (Tuple<Vector2, float> t in housesAndDistancesList)
            {
                List<Vector2> listForHouse = new List<Vector2>();
                rangeSearch(Tree, t.Item1, t.Item2, listForHouse);
                returnList.Add(listForHouse);
            }

           return returnList.AsEnumerable();
        }


        private static IEnumerable<Tuple<Vector2, Vector2>> FindRoute(Vector2 startingBuilding,
          Vector2 destinationBuilding, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
            var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
            List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
            var prevRoad = startingRoad;
            for (int i = 0; i < 30; i++)
            {
                prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, destinationBuilding)).First());
                fakeBestPath.Add(prevRoad);
            }
            return fakeBestPath;
        }

        private static IEnumerable<IEnumerable<Tuple<Vector2, Vector2>>> FindRoutesToAll(Vector2 startingBuilding,
          IEnumerable<Vector2> destinationBuildings, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
            List<List<Tuple<Vector2, Vector2>>> result = new List<List<Tuple<Vector2, Vector2>>>();
            foreach (var d in destinationBuildings)
            {
                var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
                List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
                var prevRoad = startingRoad;
                for (int i = 0; i < 30; i++)
                {
                    prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, d)).First());
                    fakeBestPath.Add(prevRoad);
                }
                result.Add(fakeBestPath);
            }
            return result;
        }
    }
#endif
}
