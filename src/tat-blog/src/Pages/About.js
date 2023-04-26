import React,{useEffect} from "react";

const About = ()=>{
    useEffect(()=>{
        document.title='About';
    },[]);

    return(
        <h1>
            Day la trang About
        </h1>
    )
}

export default About;