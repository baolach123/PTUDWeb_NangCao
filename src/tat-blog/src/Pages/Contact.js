import React,{useEffect} from "react";

const Contact = ()=>{
    useEffect(()=>{
        document.title='Contact';
    },[]);

    return(
        <h1>
            Day la trang Contact
        </h1>
    )
}

export default Contact;