import { tab } from "@testing-library/user-event/dist/tab";
import { Link } from "react-router-dom";

const TagList =({TagList})=>{
    if(TagList&& Array.isArray(TagList)&& TagList.length>0)
    return(
        <>
            {TagList.map((item,index)=>{
                return(
                    <Link to={`/blog/tag?slug=${item.name}`}
                        title={item.name}
                        className='btn btn-sm btn-outline-secondary me-1'
                        key={index}>
                            {item.name}
                    </Link>
                );
            })}
        </>
    );
    else
    return(
        <></>
    );
};

export default TagList