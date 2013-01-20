﻿[<AutoOpen>]
module FSharp.GitHubApi.ApiHelper

    open System.Net
    open Newtonsoft.Json
    open RestSharp
        

    // -------------------- //
    // Public data types    //
    // -------------------- //
    type Error = {
        [<field: JsonProperty(PropertyName="resource")>] 
        Resource : string
        [<field: JsonProperty(PropertyName="field", Required=Required.Default)>]
        Field : string
        [<field: JsonProperty(PropertyName="code", Required=Required.Default)>] 
        Code : string
    }

    type Message = {
        [<field: JsonProperty(PropertyName="message")>] 
        Message : string
        [<field: JsonProperty(PropertyName="errors", Required=Required.Default)>]
        Errors : Error array
    }

    type ResponseStatus = 
        | Completed
        | ErrorResponse of string
        | Timeout
        | Aborted

    type ResponseContent<'T> = 
        | Content of 'T
        | Error of Message
        | NoContent

    type GitHubResponse<'T> = {
        StatusCode          : HttpStatusCode
        StatusDescription   : string
        ResponseStatus      : ResponseStatus option       
        ContentRaw          : string
        Content             : ResponseContent<'T> }

    let ConvertResponse<'T> (r:IRestResponse) = 
        let map = 
            match r.ResponseStatus with 
            | RestSharp.ResponseStatus.Completed -> Some(Completed)
            | RestSharp.ResponseStatus.TimedOut -> Some(Timeout)
            | RestSharp.ResponseStatus.Aborted -> Some(Aborted)
            | RestSharp.ResponseStatus.Error -> Some(ErrorResponse(r.ErrorMessage))
            | RestSharp.ResponseStatus.None -> None
        { StatusCode          = r.StatusCode
          StatusDescription   = r.StatusDescription
          ResponseStatus      = map
          ContentRaw          = r.Content
          Content             = ResponseContent<'T>.NoContent }