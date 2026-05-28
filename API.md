---
title: Backend | v1 v1.0.0
language_tabs:
  - shell: Curl
  - http: HTTP
language_clients:
  - shell: ""
  - http: ""
toc_footers: []
includes: []
search: true
highlight_theme: darkula
headingLevel: 2

---

<!-- Generator: Widdershins v4.0.1 -->

<h1 id="backend-v1">Backend | v1 v1.0.0</h1>

> Scroll down for code samples, example requests and responses. Select a language for code samples from the tabs above or the mobile navigation menu.

Base URLs:

* <a href="http://localhost:5014/">http://localhost:5014/</a>

<h1 id="backend-v1-tasks">Tasks</h1>

## GetTasks

<a id="opIdGetTasks"></a>

> Code samples

```shell
# You can also use wget
curl -X GET http://localhost:5014/tasks

```

```http
GET http://localhost:5014/tasks HTTP/1.1
Host: localhost:5014

```

`GET /tasks`

*Get tasks*

Fetches a page of task items

<h3 id="gettasks-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|page|query|integer,string(uint32)|false|The page number to fetch|
|pageSize|query|integer,string(uint32)|false|The size of the page to fetch|

<h3 id="gettasks-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|None|

<aside class="success">
This operation does not require authentication
</aside>

## CreateTask

<a id="opIdCreateTask"></a>

> Code samples

```shell
# You can also use wget
curl -X POST http://localhost:5014/tasks \
  -H 'Content-Type: multipart/form-data'

```

```http
POST http://localhost:5014/tasks HTTP/1.1
Host: localhost:5014
Content-Type: multipart/form-data

```

`POST /tasks`

*Create task*

Creates a new task item

> Body parameter

```yaml
title: string
description: string
dueBy: 2019-08-24T14:15:22Z
itemStatus: 0

```

<h3 id="createtask-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|object|true|none|
|» *anonymous*|body|object|false|none|
|»» title|body|string|false|none|
|» *anonymous*|body|object|false|none|
|»» description|body|string|false|none|
|» *anonymous*|body|object|false|none|
|»» dueBy|body|string(date-time)|false|none|
|» *anonymous*|body|object|false|none|
|»» itemStatus|body|[TaskItemStatus](#schemataskitemstatus)|false|none|

<h3 id="createtask-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|None|

<aside class="success">
This operation does not require authentication
</aside>

## GetTask

<a id="opIdGetTask"></a>

> Code samples

```shell
# You can also use wget
curl -X GET http://localhost:5014/tasks/{id}

```

```http
GET http://localhost:5014/tasks/{id} HTTP/1.1
Host: localhost:5014

```

`GET /tasks/{id}`

*Get task*

Fetches a task by its ID number

<h3 id="gettask-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int64)|true|The ID number of the task|

<h3 id="gettask-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not Found|None|

<aside class="success">
This operation does not require authentication
</aside>

## DeleteTask

<a id="opIdDeleteTask"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE http://localhost:5014/tasks/{id}

```

```http
DELETE http://localhost:5014/tasks/{id} HTTP/1.1
Host: localhost:5014

```

`DELETE /tasks/{id}`

*Delete task*

Deletes a task

<h3 id="deletetask-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int64)|true|The ID number of the task|

<h3 id="deletetask-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|204|[No Content](https://tools.ietf.org/html/rfc7231#section-6.3.5)|No Content|None|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not Found|None|

<aside class="success">
This operation does not require authentication
</aside>

## PatchTask

<a id="opIdPatchTask"></a>

> Code samples

```shell
# You can also use wget
curl -X PATCH http://localhost:5014/tasks/{id} \
  -H 'Content-Type: application/json-patch+json' \
  -H 'Accept: application/problem+json'

```

```http
PATCH http://localhost:5014/tasks/{id} HTTP/1.1
Host: localhost:5014
Content-Type: application/json-patch+json
Accept: application/problem+json

```

`PATCH /tasks/{id}`

*Patch task*

Modifies a task using a RFC 6902 JSON PATCH document

> Body parameter

```json
[
  {
    "op": "add",
    "path": "string",
    "value": null
  }
]
```

<h3 id="patchtask-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int64)|true|The ID number of the task|
|body|body|[JsonPatchDocument](#schemajsonpatchdocument)|true|An RFC 6902 JSON Patch document to apply to the target task|

> Example responses

> 400 Response

```json
{
  "type": null,
  "title": null,
  "status": null,
  "detail": null,
  "instance": null,
  "errors": {
    "property1": [
      "string"
    ],
    "property2": [
      "string"
    ]
  }
}
```

<h3 id="patchtask-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|400|[Bad Request](https://tools.ietf.org/html/rfc7231#section-6.5.1)|Bad Request|[HttpValidationProblemDetails](#schemahttpvalidationproblemdetails)|
|404|[Not Found](https://tools.ietf.org/html/rfc7231#section-6.5.4)|Not Found|None|

<aside class="success">
This operation does not require authentication
</aside>

# Schemas

<h2 id="tocS_HttpValidationProblemDetails">HttpValidationProblemDetails</h2>
<!-- backwards compatibility -->
<a id="schemahttpvalidationproblemdetails"></a>
<a id="schema_HttpValidationProblemDetails"></a>
<a id="tocShttpvalidationproblemdetails"></a>
<a id="tocshttpvalidationproblemdetails"></a>

```json
{
  "type": null,
  "title": null,
  "status": null,
  "detail": null,
  "instance": null,
  "errors": {
    "property1": [
      "string"
    ],
    "property2": [
      "string"
    ]
  }
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|type|null,string|false|none|none|
|title|null,string|false|none|none|
|status|null,integer,string(int32)|false|none|none|
|detail|null,string|false|none|none|
|instance|null,string|false|none|none|
|errors|object|false|none|none|
|» **additionalProperties**|[string]|false|none|none|

<h2 id="tocS_JsonPatchDocument">JsonPatchDocument</h2>
<!-- backwards compatibility -->
<a id="schemajsonpatchdocument"></a>
<a id="schema_JsonPatchDocument"></a>
<a id="tocSjsonpatchdocument"></a>
<a id="tocsjsonpatchdocument"></a>

```json
[
  {
    "op": "add",
    "path": "string",
    "value": null
  }
]

```

### Properties

oneOf

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|object|false|none|none|
|» op|string|true|none|none|
|» path|string|true|none|none|
|» value|any|true|none|none|

xor

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|object|false|none|none|
|» op|string|true|none|none|
|» path|string|true|none|none|
|» from|string|true|none|none|

xor

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|object|false|none|none|
|» op|string|true|none|none|
|» path|string|true|none|none|

#### Enumerated Values

|Property|Value|
|---|---|
|op|add|
|op|replace|
|op|test|
|op|move|
|op|copy|
|op|remove|

<h2 id="tocS_TaskItemStatus">TaskItemStatus</h2>
<!-- backwards compatibility -->
<a id="schemataskitemstatus"></a>
<a id="schema_TaskItemStatus"></a>
<a id="tocStaskitemstatus"></a>
<a id="tocstaskitemstatus"></a>

```json
0

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|*anonymous*|integer|false|none|none|

