import React,{useEffect} from "react";

const RSS = ()=>{
    useEffect(()=>{
        document.title='RSS';
    },[]);

    return(
        <h1>
            Day la trang RSS
        </h1>
    )
}

export default RSS;