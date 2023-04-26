import { useState,useEffect } from "react";
import { ListGroup } from "react-bootstrap";
import { Link} from "react-router-dom";
import { getCategoryies } from "../Services/Widgets";

const CategoriesWidget = () =>{
    const [categoryList, setCategoryList] = useState([]);

    useEffect(()=>{
        getCategoryies().then(data=>{
        if(data)
            setCategoryList(data);
        else
            setCategoryList([]);
    });
},[])
return(
    <div className='mb-4'>
        <h3 className='text-success mb-2'>
            cac chu de
        </h3>
        {categoryList.length>0 && 
            <ListGroup>
                {categoryList.map((item,index)=>{
                    return(
                        <ListGroup.Item key={index}>
                            <Link to={`/blog/category?slug=${item.urlSlug}`}
                                title={item.description}
                                key= {index}>
                                    {item.name}
                                    <span>
                                        &nbsp;{item.postCount}
                                    </span>
                                </Link>
                        </ListGroup.Item>
                    )
                })}    
            </ListGroup>}
    </div>
)
}

export default CategoriesWidget;