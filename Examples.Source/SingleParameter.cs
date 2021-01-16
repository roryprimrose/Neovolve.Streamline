namespace Examples.Source
{
    using System;

    public class SingleParameter
    {
        private readonly IDoSomething _something;

        public SingleParameter(IDoSomething something)
        {
            _something = something;
        }

        public string Run(Guid id)
        {
            return _something.DoSomething(id);
        }
    }
}