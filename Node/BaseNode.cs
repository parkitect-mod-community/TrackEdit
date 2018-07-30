namespace TrackEdit.Node
{
    public class BaseNode : INode
    {
        protected int index;
        protected TrackSegment4 segment;
        
        public BaseNode(TrackSegment4 segment, int index)
        {
            this.segment = segment;
            this.index = index;
        }
        
        public bool 
    }
}